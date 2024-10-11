using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace GeoDatabase.ORM.Mapper.Mappings.Factories
{
    internal class DefaultMappingConfigsFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<Type, MappingConfig>> _createConfigDelegates
            = new ConcurrentDictionary<Type, Func<Type, MappingConfig>>();

        public MappingConfig Create(Type type, string shapeFilesDirectory)
        {
            var func = getCreateConfigFunc(type, shapeFilesDirectory);

            return func(type);
        }

        private Func<Type, MappingConfig> getCreateConfigFunc(Type type, string shapeFilesDirectory)
        {
            return _createConfigDelegates.GetOrAdd(type, t =>
            {
                var newConfigExpr = Expression.New(typeof(MappingConfig<>).MakeGenericType(t));
                var configExpr = Expression.Variable(typeof(MappingConfig<>).MakeGenericType(t));
                var assignExpr = Expression.Assign(configExpr, newConfigExpr);

                var builderType = typeof(DefaultTypeMappingBuilder<>).MakeGenericType(t);
                var newBuilderExpr = Expression.New(builderType);

                var configureMethod = builderType.GetMethod("Configure",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                var callConfigureExpr =
                    Expression.Call(newBuilderExpr, configureMethod, configExpr, Expression.Constant(shapeFilesDirectory));

                var body = Expression.Block(new ParameterExpression[] { configExpr }, assignExpr, callConfigureExpr, configExpr);
                return Expression.Lambda<Func<Type, MappingConfig>>(body, Expression.Parameter(typeof(Type)))
                    .Compile();
            });
        }
    }
}
