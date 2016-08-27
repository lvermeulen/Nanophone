using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Nanophone.Core;
using Nanophone.Fabio;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.WebApi;
using NLog;

namespace SampleService.AspNetCore.Owin
{
    public class Program
    {
        public static void Main()
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            const int PORT = 9030;
            string url = $"http://localhost:{PORT}/";
            var consulRegistryHost = new ConsulRegistryHost();
            var serviceRegistry = new ServiceRegistry(consulRegistryHost);

            // uncomment for fabio
            //serviceRegistry.ResolveServiceInstancesWith(new FabioAdapter(new Uri("http://localhost:9999")));

            serviceRegistry.AddTenant(new WebApiRegistryTenant(new Uri(url)),
                "values", "1.7.0-pre", tags: new[] { "urlprefix-/values" })
                .Wait();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{PORT}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
