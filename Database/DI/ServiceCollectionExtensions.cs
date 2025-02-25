using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Database.Repositories;
using Interfaces.Database.Repositories;
using Interfaces.Database.Abstractions;
using Database.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Database.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataBase(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            return serviceCollection.AddDbContext<AppDbContext>(opt => 
            {
                opt.UseSqlite(configuration.GetValue<string>("ConnectionString"));
                SQLitePCL.Batteries.Init();
            }, ServiceLifetime.Transient)
                .AddAsBoth<IExperimentsRepository, ExperimentsRepository>()
                .AddAsBoth<IGasRepository, GasRepository>()
                .AddAsBoth<ISceneRepository, ScenesRepository>()
                .AddAsBoth<IShipsRepository, ShipsRepository>()
                .AddAsBoth<IRoutesRepository, RoutesRepository>()
                .AddAsBoth<IRoutePointsRepository, RoutePointsRepository>()
                .AddAsBoth<IExperimentEnvironmentRepository, ExperimentEnvironmentRepository>()
                .AddAsBoth<IProfilesRepository, ProfilesRepository>()
                .AddSingleton<IRepositoriesProvider, RepositoriesProvider>()
                .AddTransient<IShapesManager, ShapesManager>()
                .AddTransient<IDbManager, DbManager>();
        }

        private static IServiceCollection AddAsBoth<TService, TImplementation>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);

            services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
            foreach (var genericInterface in serviceType.GetInterfaces().Where(i => i.IsGenericType))
            {
                services.Add(new ServiceDescriptor(genericInterface, implementationType, lifetime));
            }

            return services;
        }
    }
}
