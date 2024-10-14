using Entities;
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
using Tools;
using Expression = System.Linq.Expressions.Expression;

namespace GeoDatabase.ORM.Database
{
    internal class Database
    {
        private readonly string _shapeFilesDirectory;
        internal readonly IServiceProvider ServiceProvider;
        private readonly ILogger<Database> _logger;

        private static readonly ConcurrentDictionary<Tuple<Type, EntityState, bool>, Func<object, int>> _savers
            = new ConcurrentDictionary<Tuple<Type, EntityState, bool>, Func<object, int>>();

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
                var key = new Tuple<Type, EntityState, bool>(entry.EntityType, entry.State, entry.Shape != null);

                var saver = _savers.GetOrAdd(key, _ => getSaver(config, entry));
                entry.ShapeIndex = saver.Invoke(entry.Entity);
            }

            return config.Shapefile.StopEditingShapes();
        }

        private Func<object, int> getSaver(MappingConfig config, EntityEntry entry)
        {
            if (entry.State == EntityState.Updated)
            {
                return getUpdater(config, entry);
            }
            else
            {
                return getAddFunc(config, entry);
            }
        }

        private Func<object, int> getAddFunc(MappingConfig config, EntityEntry entry)
        {
            var param = Expression.Parameter(typeof(object));

            var shapefile = Expression.Constant(config.Shapefile);
            var shapefileType = Expression.Constant(config.Shapefile.GetType());

            var expressions = new List<Expression>();
            var shapeIndexVariable = Expression.Variable(typeof(int));

            var addShapeArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(entry.Shape ?? new Shape()));
            var callAddShape = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("EditAddShape"), Expression.Constant(BindingFlags.InvokeMethod),
                    Expression.Constant(null, typeof(Binder)), shapefile, addShapeArgs);

            foreach (var prop in entry.EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (config.IgnoredProperties.Contains(prop.Name) ||
                   !config.ColumnIndexes.TryGetValue(prop.Name, out var columnIndex) || columnIndex == -1)
                {
                    continue;
                }

                var editCellArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(columnIndex, typeof(object)), Expression.Convert(shapeIndexVariable, typeof(object)),
                    Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(param, entry.EntityType), prop), typeof(object)));
                var callEditCell = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("EditCellValue"), Expression.Constant(BindingFlags.InvokeMethod),
                    Expression.Constant(null, typeof(Binder)), shapefile, editCellArgs);
                expressions.Add(callEditCell);
            }

            expressions.Add(Expression.Assign(shapeIndexVariable, Expression.Convert(callAddShape, typeof(int))));

            expressions.Add(shapeIndexVariable);
            var body = Expression.Block(new ParameterExpression[] { shapeIndexVariable }, expressions);
            return Expression.Lambda<Func<object, int>>(body, param).Compile();
        }


        private Func<object, int> getUpdater(MappingConfig config, EntityEntry entry)
        {
            var param = Expression.Parameter(typeof(object));

            var shapefile = Expression.Constant(config.Shapefile);
            var shapefileType = Expression.Constant(config.Shapefile.GetType());

            var expressions = new List<Expression>();
            var shapeIndexVariable = Expression.Variable(typeof(int));

            expressions.Add(Expression.Assign(shapeIndexVariable, Expression.Constant(entry.ShapeIndex)));
            var shape = Expression.Variable(typeof(Shape));
            expressions.Add(Expression.Assign(shape, Expression.Constant(entry.Shape)));

            var updateShapeArgs = Expression.NewArrayInit(typeof(object), shapeIndexVariable, shape);
            var callUpdateShape = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("EditUpdateShape"), Expression.Constant(BindingFlags.InvokeMethod),
                    Expression.Constant(null, typeof(Binder)), shapefile, updateShapeArgs);

            var shapeNotProvidedClause = new List<Expression>();

            var getShapeArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(entry.ShapeIndex, typeof(object)));
            var getShape = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("Shape"), Expression.Constant(BindingFlags.GetProperty),
                    Expression.Constant(null, typeof(Binder)), shapefile, getShapeArgs);

            shapeNotProvidedClause.Add(Expression.Assign(shape, getShape));

            foreach (var prop in entry.EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (config.IgnoredProperties.Contains(prop.Name) ||
                   !config.ColumnIndexes.TryGetValue(prop.Name, out var columnIndex) || columnIndex == -1)
                {
                    continue;
                }

                var editCellArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(columnIndex, typeof(object)), Expression.Convert(shapeIndexVariable, typeof(object)),
                    Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(param, entry.EntityType), prop), typeof(object)));
                var callEditCell = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("EditCellValue"), Expression.Constant(BindingFlags.InvokeMethod),
                    Expression.Constant(null, typeof(Binder)), shapefile, editCellArgs);
                shapeNotProvidedClause.Add(callEditCell);
            }

            var setCoords = new List<Expression>();
            if (TypeTools.Implements(entry.Entity, typeof(IEntityWithCoordinates)))
            {
                var point = Expression.Variable(typeof(Point));

                var getPointArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(0, typeof(object)));
                var getPoint = Expression.Call(Expression.Constant(typeof(Shape)), _invokeMemberMethod,
                        Expression.Constant("Point"), Expression.Constant(BindingFlags.GetProperty),
                        Expression.Constant(null, typeof(Binder)), shapefile, getPointArgs);

                setCoords.Add(point);
                setCoords.Add(Expression.Assign(point, getPoint));

                var setXArgs = Expression.NewArrayInit(typeof(object), Expression.Convert(
                    Expression.PropertyOrField(Expression.Constant(entry.Entity), "X"), typeof(object)));
                
                var setX = Expression.Call(Expression.Constant(typeof(Point)), _invokeMemberMethod,
                        Expression.Constant("x"), Expression.Constant(BindingFlags.SetProperty),
                        Expression.Constant(null, typeof(Binder)), point, setXArgs);

                var setYArgs = Expression.NewArrayInit(typeof(object), Expression.Convert(
                    Expression.PropertyOrField(Expression.Constant(entry.Entity), "Y"), typeof(object)));
                
                var setY = Expression.Call(Expression.Constant(typeof(Point)), _invokeMemberMethod,
                        Expression.Constant("y"), Expression.Constant(BindingFlags.SetProperty),
                        Expression.Constant(null, typeof(Binder)), point, setYArgs);

                setCoords.Add(setX);
                setCoords.Add(setY);
            }
            
            var condition = Expression.NotEqual(Expression.Constant(entry.Shape), Expression.Constant(null, entry.Shape.GetType()));

            var trueBlock = Expression.Block(setCoords.Prepend(updateShapeArgs).ToArray());
            var falseBlock = Expression.Block(shapeNotProvidedClause.Concat(setCoords).ToArray());

            expressions.Add(Expression.IfThenElse(condition, trueBlock, falseBlock));

            expressions.Add(shapeIndexVariable);
            var body = Expression.Block(new ParameterExpression[] { shapeIndexVariable }, expressions);
            return Expression.Lambda<Func<object, int>>(body, param).Compile();
        }
    }
}
