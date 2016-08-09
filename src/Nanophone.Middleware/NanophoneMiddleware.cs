using System;
using Microsoft.Extensions.DependencyInjection;
using Nanophone.Core;

namespace Nanophone.Middleware
{
    public class NanophoneMiddleware
    {
        public NanophoneMiddleware(IServiceCollection services, Func<IRegistryHost> registryHostFactory)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (registryHostFactory == null)
            {
                throw new ArgumentNullException(nameof(registryHostFactory));
            }

            var registryHost = registryHostFactory();
            var serviceRegistry = new ServiceRegistry(registryHost);

            services.AddSingleton(serviceRegistry);
            var serviceDescriptor = new ServiceDescriptor(typeof(ServiceRegistry), typeof(ServiceRegistry), ServiceLifetime.Singleton);
            services.Add(serviceDescriptor);
        }
    }
}
