using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.Nancy;
using Nito.AsyncEx;
using NLog;

namespace SampleService.Nancy.Kestrel
{
    public class Program
    {
        private static async Task MainAsync()
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            const int PORT = 9060;
            string url = $"http://localhost:{PORT}/";
            var consulRegistryHost = new ConsulRegistryHost();
            var serviceRegistry = new ServiceRegistry(consulRegistryHost);

            // uncomment for fabio
            //serviceRegistry.ResolveServiceInstancesWith(new FabioAdapter(new Uri("http://localhost:9999")));

            await serviceRegistry.AddTenantAsync(new NancyRegistryTenant(new Uri(url)),
                "orders", "1.3.4", tags: new[] { "urlprefix-/orders" });

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{PORT}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        public static void Main()
        {
            AsyncContext.Run(async () => await MainAsync());
        }
    }
}
