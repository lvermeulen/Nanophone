using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.WebApi;
using NLog;

namespace SampleService.Nancy.Kestrel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const bool USING_FABIO = false;
            const bool IGNORE_CRITICAL_SERVICES = true;

            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            const int PORT = 9060;
            string url = $"http://localhost:{PORT}/";
            var serviceRegistry = new ServiceRegistry();
            var consulConfiguration = USING_FABIO
                ? new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES, FabioUri = new Uri("http://localhost:9999") }
                : new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES };
            serviceRegistry.Start(new WebApiRegistryTenant(new Uri(url)), new ConsulRegistryHost(consulConfiguration),
                USING_FABIO ? "values v1.7-pre" : "values", "1.7-pre", relativePaths: new[] { "/values" });

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
