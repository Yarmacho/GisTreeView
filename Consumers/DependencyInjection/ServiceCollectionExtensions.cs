using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Consumers.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsumers(this IServiceCollection services)
        {


            return services;
        }
    }
}
