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
        public static RegistryInformation AddTenant(this IApplicationBuilder app, IRegistryTenant registryTenant, string serviceName, string version, Uri healthCheckUri = null, IEnumerable<string> tags = null)
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
            var registryInformation = serviceRegistry.AddTenantAsync(registryTenant, serviceName, version, healthCheckUri, tags)
                .Result;

            return registryInformation;
        }

        public static string AddHealthCheck(this IApplicationBuilder app, RegistryInformation registryInformation, Uri checkUri, TimeSpan? interval = null, string notes = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (registryInformation == null)
            {
                throw new ArgumentNullException(nameof(registryInformation));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<ServiceRegistry>();
            string checkId = serviceRegistry.AddHealthCheckAsync(registryInformation.Name, registryInformation.Id, checkUri, interval, notes)
                .Result;

            return checkId;
        }
    }
}
