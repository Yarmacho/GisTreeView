using GeoDatabase.ORM.Mapper;
using GeoDatabase.ORM.Mapper.Mappings;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Expression = System.Linq.Expressions.Expression;

namespace GeoDatabase.ORM.Database
{
    internal class Database
    {
        private readonly string _shapeFilesDirectory;
        internal readonly IServiceProvider ServiceProvider;
        private readonly ILogger<Database> _logger;

        private static readonly ConcurrentDictionary<Type, Action<object>> _savers
            = new ConcurrentDictionary<Type, Action<object>>();

        private static readonly MethodInfo _invokeMemberMethod
            = typeof(Type).GetMethod("InvokeMember", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string), typeof(BindingFlags), typeof(Binder), typeof(object), typeof(object[]) }, null);

        public Database(string shapeFilesDirectory, IServiceProvider serviceProvider)
        {
            _shapeFilesDirectory = shapeFilesDirectory;
            ServiceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<Database>>();
        }

        public bool EnsureShapefilesStructure()
        {
            if (!Directory.Exists(_shapeFilesDirectory))
            {
                _logger.LogError("Shapes directory not found (\"{ShapesDirectory}\")", _shapeFilesDirectory);
                return false;
            }

            var mappings = ServiceProvider.GetRequiredService<MappingConfigs>();
            foreach (var mapping in mappings)
            {
                if (mapping.Shapefile == null)
                {
                    _logger.LogError("Failed to load \"{ShapesfileName}\"", mapping.ShapefileName);
                    return false;
                }

                var propertiesToCreate = mapping.ColumnIndexes
                    .Where(c => c.Value == -1)
                    .Select(c => c.Key)
                    .ToHashSet();

                if (propertiesToCreate.Count > 0)
                {
                    mapping.Shapefile.StartEditingTable();

                    var entityType = mapping.GetType().GetGenericArguments()[0];
                    foreach (var propertyName in propertiesToCreate)
                    {
                        var property = entityType.GetProperty(propertyName);
                        if (property == null)
                        {
                            throw new Exception("Invalid mapping property");
                        }

                        if (mapping.ColumnNames.TryGetValue(propertyName, out var columnName))
                        {
                            throw new Exception("Invalid property");
                        }

                        if (!mapping.ColumnPrecisions.TryGetValue(propertyName, out var precision))
                        {
                            precision = getDefaultFieldPrecision(property.PropertyType);
                        }

                        if (!mapping.ColumnLengths.TryGetValue(propertyName, out var length))
                        {
                            length = getDefaultFieldLength(property.PropertyType);
                        }

                        var fieldType = getFieldType(property.PropertyType);

                        mapping.Shapefile.EditAddField(columnName, fieldType, precision, length);
                    }

                    mapping.Shapefile.StopEditingTable();
                }
            }

            return true;
        }

        private int getDefaultFieldLength(Type type)
        {
            return type == typeof(double) || type == typeof(decimal) || type == typeof(float)
                ? 16
                : type == typeof(int)
                    ? 10
                    : type == typeof(string)
                        ? 200
                        : 1;
        }

        private int getDefaultFieldPrecision(Type type)
        {
            return type == typeof(double) || type == typeof(decimal) || type == typeof(float)
                ? 4
                : 0;
        }

        private FieldType getFieldType(Type type)
        {
            return type == typeof(double) || type == typeof(decimal) || type == typeof(float)
                ? FieldType.DOUBLE_FIELD
                : type == typeof(int)
                    ? FieldType.INTEGER_FIELD
                    : type == typeof(string)
                        ? FieldType.STRING_FIELD
                        : type == typeof(bool)
                            ? FieldType.BOOLEAN_FIELD
                            : throw new ArgumentException("Invalid property type");
        }

        public bool LoadEntity<T>(EntityEntry<T> entry)
        {
            return LoadEntity(entry);
        }

        public bool LoadEntity(EntityEntry entry)
        {
            var mappings = ServiceProvider.GetRequiredService<MappingConfigs>();
            var config = mappings.GetConfig(entry.EntityType);

            config.Shapefile.StartEditingShapes();
            if (entry.State == EntityState.Removed)
            {
                config.Shapefile.EditDeleteShape(entry.ShapeIndex);
            }
            else
            {
                var saver = _savers.GetOrAdd(entry.EntityType, _ => getSaver(config, entry.EntityType, entry.ShapeIndex, entry.Shape));
                saver.Invoke(entry.Entity);
            }

            return config.Shapefile.StopEditingShapes();
        }

        private Action<object> getSaver(MappingConfig config, Type type, int shapeIndex, Shape shape = null)
        {
            var param = Expression.Parameter(typeof(object));

            var shapefile = Expression.Constant(config.Shapefile);
            var shapefileType = Expression.Constant(config.Shapefile.GetType());

            var expressions = new List<Expression>();
            var shapeIndexVariable = Expression.Variable(typeof(int));
            if (shapeIndex != -1)
            {
                expressions.Add(Expression.Assign(shapeIndexVariable, Expression.Constant(shapeIndex)));
            }
            else
            {
                var addShapeArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(shape ?? new Shape()));
                var callAddShape = Expression.Call(shapefileType, _invokeMemberMethod,
                        Expression.Constant("EditAddShape"), Expression.Constant(BindingFlags.InvokeMethod),
                        Expression.Constant(null, typeof(Binder)), shapefile, addShapeArgs);

                expressions.Add(Expression.Assign(shapeIndexVariable, Expression.Convert(callAddShape, typeof(int))));
            }

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (config.IgnoredProperties.Contains(prop.Name) ||
                   !config.ColumnIndexes.TryGetValue(prop.Name, out var columnIndex) || columnIndex == -1)
                {
                    continue;
                }

                var editCellArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(columnIndex, typeof(object)), Expression.Convert(shapeIndexVariable, typeof(object)),
                    Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(param, type), prop), typeof(object)));
                var callEditCell = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("EditCellValue"), Expression.Constant(BindingFlags.InvokeMethod),
                    Expression.Constant(null, typeof(Binder)), shapefile, editCellArgs);
                expressions.Add(callEditCell);
            }

            var body = Expression.Block(new ParameterExpression[] { shapeIndexVariable }, expressions);
            return Expression.Lambda<Action<object>>(body, param).Compile();
        }
    }
}
