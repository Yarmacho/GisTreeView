using GeoDatabase.ORM.Mapper.Mappings.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace GeoDatabase.ORM.Mapper.Mappings
{
    internal class MappingsInitializer
    {
        private static readonly ConcurrentDictionary<Type, Action<MappingConfigs, TypeMapping>> _addConfigDelegate
            = new ConcurrentDictionary<Type, Action<MappingConfigs, TypeMapping>>();

        private readonly IServiceProvider _serviceProvider;

        public MappingsInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MappingConfigs Init(string shapeFilesDirectory)
        {
            var mappingConfigs = new MappingConfigs(shapeFilesDirectory);
            foreach (var typeMapping in _serviceProvider.GetServices<TypeMapping>())
            {
                var func = _addConfigDelegate.GetOrAdd(typeMapping.GetType(),
                    t => createAddConfigDelegate(typeMapping, shapeFilesDirectory));
                func.Invoke(mappingConfigs, typeMapping);
            }

            return mappingConfigs;
        }

        private static Action<MappingConfigs, TypeMapping> createAddConfigDelegate(TypeMapping typeMapping, string shapeFilesDirectory)
        {
            var mappingType = typeMapping.GetType();
            var argument = getGenericTypeMappingArgument(mappingType);
            if (argument == null)
            {
                throw new Exception("Invalid type mapping");
            }

            var configsParameter = Expression.Parameter(typeof(MappingConfigs));
            var typeMappingConfig = Expression.Parameter(typeof(TypeMapping));

            var variable = Expression.Variable(typeof(MappingConfig<>).MakeGenericType(argument));
            var newConfig = Expression.New(typeof(MappingConfig<>).MakeGenericType(argument));
            var assign = Expression.Assign(variable, newConfig);

            var configureMethod = mappingType.GetMethod("Configure", BindingFlags.Instance | BindingFlags.NonPublic);
            var configure = Expression.Call(Expression.Convert(typeMappingConfig, mappingType), configureMethod, variable, Expression.Constant(shapeFilesDirectory));

            var addConfigMethod = configsParameter.Type.GetMethod("AddConfig", new Type[] { typeof(MappingConfig) });
            var addConfig = Expression.Call(configsParameter, addConfigMethod, variable);

            var body = Expression.Block(new ParameterExpression[] { variable }, assign, configure, addConfig);

            return Expression
                .Lambda<Action<MappingConfigs, TypeMapping>>(body, configsParameter, typeMappingConfig)
                .Compile();
        }

        private static Type getGenericTypeMappingArgument(Type type)
        {
            var returnType = type;
            while (!returnType.IsGenericType && returnType != null)
            {
                returnType = returnType.BaseType;
            }

            return returnType == null || !returnType.IsGenericType ? null : returnType.GetGenericArguments()[0];
        }
    }
}
