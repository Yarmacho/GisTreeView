using Entities.Entities;
using Microsoft.Extensions.DependencyInjection;
using WindowsFormsApp4.ShapeConverters;

namespace DynamicForms.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddShapeConverters(this IServiceCollection services)
        {
            services.AddTransient<IShapeEntityConverter<Gas>, ShapeToGasConverter>();
            services.AddTransient<IShapeEntityConverter<Scene>, ShapeToSceneConverter>();
            services.AddTransient<IShapeEntityConverter<Ship>, ShapeToShipConverter>();
            services.AddTransient<IShapeEntityConverter<Route>, ShapeToRouteConverter>();

            return services;
        }
    }
}
