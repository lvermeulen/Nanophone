using System;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.Nancy;
using NLog;

namespace SampleService.Nancy.netcore
{
    public class Program
    {
        static void Main()
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            Console.WriteLine("Press ENTER to exit");

            var serviceRegistry = new ServiceRegistry();
            serviceRegistry.Start(new NancyRegistryTenant(new Uri("http://localhost:9060")), new ConsulRegistryHost(),
                "orders", "v3", relativePaths: new[] { "/orders" });

            Console.ReadLine();
        }
    }
}
