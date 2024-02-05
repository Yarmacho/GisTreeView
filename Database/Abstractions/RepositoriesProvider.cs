using Interfaces.Database.Abstractions;
using Interfaces.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Database.Abstractions
{
    internal class RepositoriesProvider : IRepositoriesProvider
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<Type, Type> _cache = new ConcurrentDictionary<Type, Type>();

        public RepositoriesProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Get<T>() where T : IRepository
        {
            var repositoryType = typeof(T);
            if (_cache.TryGetValue(repositoryType, out var implementationType))
            {
                return (T)_serviceProvider.GetRequiredService(implementationType);
            }

            try
            {
                var repository = _serviceProvider.GetRequiredService<T>();
                _cache.TryAdd(repositoryType, repositoryType);

                return repository;
            }
            catch
            {
                var types = GetType().Assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.GetInterfaces().Any(i => i == repositoryType))
                    {
                        _cache.TryAdd(repositoryType, type);
                        return (T)_serviceProvider.GetService(type);
                    }
                }

                throw new ArgumentException();
            }
        }
    }
}
