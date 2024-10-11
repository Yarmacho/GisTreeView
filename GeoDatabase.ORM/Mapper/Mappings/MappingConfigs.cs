using GeoDatabase.ORM.Mapper.Mappings;
using GeoDatabase.ORM.Mapper.Mappings.Factories;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GeoDatabase.ORM.Mapper
{
    internal class MappingConfigs : IEnumerable<MappingConfig>, IMappingConfigProvider
    {
        private static readonly ConcurrentDictionary<Type, MappingConfig> _configs
            = new ConcurrentDictionary<Type, MappingConfig>();

        private readonly string _shapeFilesDirectory;

        public MappingConfigs(string shapeFilesDirectory)
        {
            _shapeFilesDirectory = shapeFilesDirectory;
        }

        public void AddConfig<T>(MappingConfig<T> config)
        {
            _configs.TryAdd(typeof(T), config);
        }

        public void AddConfig(MappingConfig config)
        {
            var genericArgument = config.GetType().GetGenericArguments()[0];
            _configs.TryAdd(genericArgument, config);
        }

        public MappingConfig<T> GetConfig<T>()
        {
            return (MappingConfig<T>)GetConfig(typeof(T));
        }

        public MappingConfig GetConfig(Type type)
        {
            return _configs.GetOrAdd(type, t => new DefaultMappingConfigsFactory().Create(t, _shapeFilesDirectory));
        }

        public IEnumerator<MappingConfig> GetEnumerator()
        {
            return _configs.Values.GetEnumerator();
        }

        IMappingConfig<T> IMappingConfigProvider.GetConfig<T>()
        {
            return GetConfig<T>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
