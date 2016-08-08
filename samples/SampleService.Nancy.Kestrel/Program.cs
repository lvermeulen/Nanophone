using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Nanophone.Core;
using Nanophone.Fabio;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.Nancy;
using NLog;

namespace SampleService.Nancy.Kestrel
{
    public class Program
    {
        public static void Main()
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            const int PORT = 9060;
            string url = $"http://localhost:{PORT}/";
            var consulRegistryHost = new ConsulRegistryHost();
            var serviceRegistry = new ServiceRegistry(consulRegistryHost);

            // uncomment for fabio
            //serviceRegistry.ResolveServiceInstancesWith(new FabioAdapter(new Uri("http://localhost:9999")));

            serviceRegistry.AddTenant(new NancyRegistryTenant(new Uri(url)),
                "orders", "1.3.4", keyValuePairs: new[] { new KeyValuePair<string, string>("urlprefix-", "/orders") })
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
