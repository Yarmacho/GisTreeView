using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Database.Repositories;
using Interfaces.Database.Repositories;
using Interfaces.Database.Abstractions;
using Database.Abstractions;
using Microsoft.Extensions.Configuration;

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
                .AddSingleton<IRepositoriesProvider, RepositoriesProvider>()
                .AddTransient<IDbManager, DbManager>();
        }

        private static IServiceCollection AddAsBoth<TService, TImplementation>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TService : class
            where TImplementation : class, TService
        {
            services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
            services.Add(new ServiceDescriptor(typeof(TImplementation), typeof(TImplementation), lifetime));

            return services;
        }
    }
}
