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

        private static readonly ConcurrentDictionary<Tuple<Type, EntityState>, Func<EntityEntry, int>> _savers
            = new ConcurrentDictionary<Tuple<Type, EntityState>, Func<EntityEntry, int>>();

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
                    .Where(c => c.Value == -1 && !mapping.IgnoredProperties.Contains(c.Key))
                    .Select(c => c.Key)
                    .ToHashSet();

                if (propertiesToCreate.Count > 0)
                {
                    if (!mapping.Shapefile.StartEditingTable())
                    {
                        break;
                    }

                    var entityType = mapping.GetType().GetGenericArguments()[0];
                    foreach (var propertyName in propertiesToCreate)
                    {
                        var property = entityType.GetProperty(propertyName);
                        if (property == null)
                        {
                            throw new Exception("Invalid mapping property");
                        }

                        if (!mapping.ColumnNames.TryGetValue(propertyName, out var columnName))
                        {
                            columnName = propertyName;
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

                        var fieldIndex = mapping.Shapefile.EditAddField(columnName, fieldType, precision, length);
                        mapping.ColumnIndexes[propertyName] = fieldIndex;
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
                var key = new Tuple<Type, EntityState>(entry.EntityType, entry.State);

                var saver = _savers.GetOrAdd(key, _ => getSaver(config, entry));

                if (entry.Shape == null)
                {
                    entry.Shape = new Shape();
                }

                entry.ShapeIndex = saver.Invoke(entry);
            }

            return config.Shapefile.StopEditingShapes();
        }

        private Func<EntityEntry, int> getSaver(MappingConfig config, EntityEntry entry)
        {
            if (entry.State == EntityState.Updated)
            {
                return getUpdater(config, entry.EntityType);
            }
            else
            {
                return getAddFunc(config, entry.EntityType);
            }
        }

        private Func<EntityEntry, int> getAddFunc(MappingConfig config, Type entityType)
        {
            var param = Expression.Parameter(typeof(EntityEntry));

            var shapefile = Expression.Constant(config.Shapefile);
            var shapefileType = Expression.Constant(config.Shapefile.GetType());

            var expressions = new List<Expression>();
            var shapeIndexVariable = Expression.Variable(typeof(int));

            var addShapeArgs = Expression.NewArrayInit(typeof(object), Expression.Convert(Expression.PropertyOrField(param, "Shape"), typeof(object)));
            var callAddShape = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("EditAddShape"), Expression.Constant(BindingFlags.InvokeMethod),
                    Expression.Constant(null, typeof(Binder)), shapefile, addShapeArgs);

            expressions.Add(Expression.Assign(shapeIndexVariable, Expression.Convert(callAddShape, typeof(int))));

            var castedEntity = Expression.Convert(Expression.PropertyOrField(param, "Entity"), entityType);
            foreach (var prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (config.IgnoredProperties.Contains(prop.Name) ||
                   !config.ColumnIndexes.TryGetValue(prop.Name, out var columnIndex) || columnIndex == -1)
                {
                    continue;
                }

                var editCellArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(columnIndex, typeof(object)), 
                    Expression.Convert(shapeIndexVariable, typeof(object)),
                    Expression.Convert(Expression.MakeMemberAccess(castedEntity, prop), typeof(object)));
                var callEditCell = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("EditCellValue"), Expression.Constant(BindingFlags.InvokeMethod),
                    Expression.Constant(null, typeof(Binder)), shapefile, editCellArgs);
                expressions.Add(callEditCell);
            }

            expressions.Add(shapeIndexVariable);
            var body = Expression.Block(new ParameterExpression[] { shapeIndexVariable }, expressions);
            return Expression.Lambda<Func<EntityEntry, int>>(body, param).Compile();
        }


        private Func<EntityEntry, int> getUpdater(MappingConfig config, Type entityType)
        {
            var shapeVariable = Expression.Variable(typeof(Shape), "shape");
            var shapeIndexVariable = Expression.Variable(typeof(int));
            var variables = new List<ParameterExpression>()
            {
                shapeVariable,
                shapeIndexVariable
            };

            var param = Expression.Parameter(typeof(EntityEntry));

            var shapefile = Expression.Constant(config.Shapefile);
            var shapefileType = Expression.Constant(config.Shapefile.GetType());

            var expressions = new List<Expression>
            {
                Expression.Assign(shapeIndexVariable, Expression.PropertyOrField(param, "ShapeIndex"))
            };

            var getShapeArgs = Expression.NewArrayInit(typeof(object), Expression.Convert(Expression.PropertyOrField(param, "ShapeIndex"), typeof(object)));
            var getShape = Expression.Convert(Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("Shape"), Expression.Constant(BindingFlags.GetProperty),
                    Expression.Constant(null, typeof(Binder)), shapefile, getShapeArgs), typeof(Shape));

            var condition = Expression.Equal(Expression.PropertyOrField(param, "ShapeIndex"), Expression.Constant(-1));

            expressions.Add(Expression.Assign(shapeVariable, Expression.Condition(condition, getShape, Expression.PropertyOrField(param, "Shape"))));

            var castedEntity = Expression.Convert(Expression.PropertyOrField(param, "Entity"), entityType);
            foreach (var prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (config.IgnoredProperties.Contains(prop.Name) ||
                   !config.ColumnIndexes.TryGetValue(prop.Name, out var columnIndex) || columnIndex == -1)
                {
                    continue;
                }

                var editCellArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(columnIndex, typeof(object)), Expression.Convert(shapeIndexVariable, typeof(object)),
                    Expression.Convert(Expression.MakeMemberAccess(castedEntity, prop), typeof(object)));
                var callEditCell = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("EditCellValue"), Expression.Constant(BindingFlags.InvokeMethod),
                    Expression.Constant(null, typeof(Binder)), shapefile, editCellArgs);
                expressions.Add(callEditCell);
            }

            //if (TypeTools.Implements(entityType, typeof(IEntityWithCoordinates)))
            //{
            //    var point = Expression.Variable(typeof(Point));
            //    variables.Add(point);

            //    var getPointArgs = Expression.NewArrayInit(typeof(object), Expression.Constant(0, typeof(object)));
            //    var getPoint = Expression.Convert(Expression.Call(Expression.Constant(typeof(Shape)), _invokeMemberMethod,
            //            Expression.Constant("Point"), Expression.Constant(BindingFlags.GetProperty),
            //            Expression.Constant(null, typeof(Binder)), shapefile, getPointArgs), typeof(Point));

            //    expressions.Add(point);
            //    expressions.Add(Expression.Assign(point, getPoint));

            //    var setXArgs = Expression.NewArrayInit(typeof(object), Expression.Convert(
            //        Expression.PropertyOrField(castedEntity, "X"), typeof(object)));
                
            //    var setX = Expression.Call(Expression.Constant(typeof(Point)), _invokeMemberMethod,
            //            Expression.Constant("x"), Expression.Constant(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty),
            //            Expression.Constant(Type.DefaultBinder, typeof(Binder)), point, setXArgs);

            //    var setYArgs = Expression.NewArrayInit(typeof(object), Expression.Convert(
            //        Expression.PropertyOrField(castedEntity, "Y"), typeof(object)));
                
            //    var setY = Expression.Call(Expression.Constant(typeof(Point)), _invokeMemberMethod,
            //            Expression.Constant("y"), Expression.Constant(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty),
            //            Expression.Constant(Type.DefaultBinder, typeof(Binder)), point, setYArgs);

            //    expressions.Add(setX);
            //    expressions.Add(setY);
            //}

            expressions.Add(shapeIndexVariable);
            var body = Expression.Block(variables.ToArray(), expressions);
            return Expression.Lambda<Func<EntityEntry, int>>(body, param).Compile();
        }
    }
}
