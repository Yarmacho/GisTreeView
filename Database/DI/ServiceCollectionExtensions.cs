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
                .AddTransient<IExperimentsRepository, ExperimentsRepository>()
                .AddSingleton<IRepositoriesProvider, RepositoriesProvider>()
                .AddTransient<IDbManager, DbManager>();
        }
    }
}
