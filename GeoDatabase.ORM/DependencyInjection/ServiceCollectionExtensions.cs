using GeoDatabase.ORM.Mapper;
using GeoDatabase.ORM.Mapper.Mappings;
using GeoDatabase.ORM.QueryBuilder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GeoDatabase.ORM.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGeoDataBase(this IServiceCollection serviceCollection, string shapeFilesDirectory)
        {
            return serviceCollection.AddScoped<GeoDbContext>(serviceProvider 
                => new GeoDbContext(shapeFilesDirectory, serviceProvider))
                .AddSingleton<IShapesQueryProvider, ShapesQueryProvider>()
                .AddScoped<ChangeTracker>()
                .AddSingleton<Database.Database>(sp => new Database.Database(shapeFilesDirectory, sp));
        }

        public static IServiceCollection AddMappings(this IServiceCollection serviceCollection, Assembly assembly,
            string shapeFilesDirectory)
        {
            var mappingType = typeof(TypeMapping<>);
            foreach (var type in assembly.DefinedTypes)
            {
                if (mappingType.BaseType.IsAssignableFrom(type))
                {
                    var genericType = type.BaseType.GetGenericArguments()[0];
                    serviceCollection.AddScoped(mappingType.MakeGenericType(genericType), type);
                    serviceCollection.AddScoped(mappingType.BaseType, type);
                }
            }

            return serviceCollection
                .AddScoped<MappingConfigs>(sp => new MappingsInitializer(sp).Init(shapeFilesDirectory))
                .AddScoped<IMappingConfigProvider>(sp => sp.GetRequiredService<MappingConfigs>())
                .AddScoped<IShapesMapper, ShapesMapper>();
        }
    }
}
