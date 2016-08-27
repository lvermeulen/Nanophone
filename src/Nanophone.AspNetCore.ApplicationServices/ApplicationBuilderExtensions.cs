using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nanophone.Core;

namespace Nanophone.AspNetCore.ApplicationServices
{
    public static class ApplicationBuilderExtensions
    {
        // TODO: remove IRegistryTenant parameter
        public static IApplicationBuilder AddTenant(this IApplicationBuilder app, IRegistryTenant registryTenant, string serviceName, string version, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (registryTenant == null)
            {
                throw new ArgumentNullException(nameof(registryTenant));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<ServiceRegistry>();
            serviceRegistry.AddTenant(registryTenant, serviceName, version, healthCheckUri, tags)
                .Wait();

            return app;
        }
    }
}
