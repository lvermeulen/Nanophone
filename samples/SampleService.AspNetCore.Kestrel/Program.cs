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
        public static void Main(string[] args)
        {
            const bool USING_FABIO = false;
            const bool IGNORE_CRITICAL_SERVICES = true;

            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            const int PORT = 9030;
            string url = $"http://localhost:{PORT}/";
            var serviceRegistry = new ServiceRegistry();
            var consulConfiguration = new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES };
            var consulRegistryHost = new ConsulRegistryHost(consulConfiguration);

            // XXX
            if (USING_FABIO)
            {
                var fabioHandler = new FabioAdapter(new Uri("http://localhost:9999"));
                serviceRegistry.ResolveServiceInstancesWith(fabioHandler);
                consulRegistryHost.AddBeforeRegistrationHandler(fabioHandler);
            }

            serviceRegistry.Start(new WebApiRegistryTenant(new Uri(url)), consulRegistryHost,
                "values", "1.7-pre", keyValuePairs: new[] { new KeyValuePair<string, string>("urlprefix-", "/values") });

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{PORT}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
