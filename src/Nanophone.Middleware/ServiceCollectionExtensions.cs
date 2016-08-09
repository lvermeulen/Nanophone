using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Nanophone.Core;

namespace Nanophone.Middleware
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNanophoneRegistryTenant(this IServiceCollection services, IRegistryTenant registryTenant, string serviceName, string version, Uri healthCheckUri = null, IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
        {
            var serviceProvider = services.BuildServiceProvider();
            var serviceRegistry = serviceProvider.GetRequiredService<ServiceRegistry>();
            serviceRegistry.AddTenant(registryTenant, serviceName, version, healthCheckUri, keyValuePairs)
                .Wait();

            return services;
        }
    }
}
