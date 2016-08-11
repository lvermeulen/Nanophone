using System;
using Microsoft.Extensions.DependencyInjection;
using Nanophone.Core;

namespace Nanophone.AspNetCore.ApplicationServices
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNanophone(this IServiceCollection services, Func<IRegistryHost> registryHostFactory)
        {
            var registryHost = registryHostFactory();
            var serviceRegistry = new ServiceRegistry(registryHost);
            services.AddSingleton(serviceRegistry);

            return services;
        }
    }
}
