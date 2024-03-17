using GeoDatabase.ORM.Mapper.Mappings;
using MapWinGIS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Tools;
using Expression = System.Linq.Expressions.Expression;

namespace GeoDatabase.ORM.Mapper
{
    internal class ShapesMapper : IShapesMapper
    {
        private readonly MappingConfigs _configs;
        private static readonly ConcurrentDictionary<Type, Func<int, object>> _entityFactories
            = new ConcurrentDictionary<Type, Func<int, object>>();

        private static readonly MethodInfo _invokeMemberMethod
            = typeof(Type).GetMethod("InvokeMember", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string), typeof(BindingFlags), typeof(Binder), typeof(object), typeof(object[]) }, null);
        private static readonly MethodInfo _convertValueMethod
            = typeof(TypeTools).GetMethod("Convert", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(object) }, null);

        public ShapesMapper(MappingConfigs configs)
        {
            _configs = configs;
        }

        public T Map<T>(int shapeIndex) where T : new()
        {
            return (T)Map(shapeIndex, typeof(T));
        }

        public object Map(int shapeIndex, Type destinationType)
        {
            var config = _configs.GetConfig(destinationType);

            var factoryMethod = _entityFactories.GetOrAdd(destinationType, getFactoryMethod(config, destinationType));

            return factoryMethod?.Invoke(shapeIndex);
        }

        private Func<int, object> getFactoryMethod(MappingConfig config, Type destinationType)
        {
            var shapeIndex = Expression.Parameter(typeof(int));

            var obj = Expression.Variable(destinationType, "obj");
            var assign = Expression.Assign(obj, Expression.New(destinationType));
            var expressions = new List<Expression>() 
            {
                obj, assign
            };

            var shapefile = Expression.Constant(config.Shapefile);
            var shapefileType = Expression.Constant(config.Shapefile.GetType());
            foreach (var property in destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (config.IgnoredProperties.Contains(property.Name) ||
                    !config.ColumnIndexes.TryGetValue(property.Name, out var columnIndex) ||
                    columnIndex == -1)
                {
                    continue;
                }

                var field = Expression.NewArrayInit(typeof(object), Expression.Constant(columnIndex, typeof(object)), Expression.Convert(shapeIndex, typeof(object)));

                var cellValue = Expression.Call(shapefileType, _invokeMemberMethod,
                    Expression.Constant("CellValue"), Expression.Constant(BindingFlags.GetProperty),
                    Expression.Constant(null, typeof(Binder)), shapefile, field);
                //var value = Expression.Property(Expression.Constant(cellValue), "Item", Expression.Constant(columnIndex), Expression.Constant(shapeIndex));
                var convertedValue = Expression.Call(_convertValueMethod.MakeGenericMethod(property.PropertyType),
                    Expression.Convert(cellValue, typeof(object)));
                expressions.Add(Expression.Assign(Expression.Property(obj, property.Name), convertedValue));
            }

            expressions.Add(obj);
            var body = Expression.Block(new ParameterExpression[] { obj }, expressions.ToArray());

            return Expression.Lambda<Func<int, object>>(body, shapeIndex).Compile();
        }

        internal void UpdateTypeCreation(Type type)
        {
            if (_entityFactories.ContainsKey(type))
            {
                var config = _configs.GetConfig(type);
                _entityFactories[type] = getFactoryMethod(config, type);
            }
        }
    }
}
