using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;

namespace Nanophone.Middleware.Tests
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, LoggerFactory loggerFactory)
        {
            var registryHost = new InMemoryRegistryHost
            {
                ServiceInstances = new List<RegistryInformation>
                {
                    new RegistryInformation
                    {
                        Name = nameof(NanophoneMiddlewareShould)
                    }
                }
            };

            app.UseNanophone(() => registryHost);
        }
    }
}
