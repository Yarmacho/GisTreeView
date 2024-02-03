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

        public RepositoriesProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Get<T>() where T : IRepository
        {
            var repositoryType = typeof(T);
            try
            {
                return _serviceProvider.GetService<T>();
            }
            catch
            {
                var types = GetType().Assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.GetInterfaces().Any(i => i == repositoryType))
                    {
                        return (T)_serviceProvider.GetService(type);
                    }
                }

                throw new ArgumentException();
            }
        }
    }
}
