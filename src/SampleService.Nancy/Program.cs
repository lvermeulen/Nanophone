using System;
using System.Collections.Generic;
using System.Linq;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.Nancy;
using NLog;

namespace SampleService.Nancy
{
    class Program
    {
        static void Main()
        {
            const bool USING_FABIO = true;
            const bool IGNORE_CRITICAL_SERVICES = true;

            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            Console.WriteLine("Press ENTER to exit");

            var serviceRegistry = new ServiceRegistry();
            var consulConfiguration = USING_FABIO
                ? new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES, FabioUri = new Uri("http://localhost:9999") }
                : new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES };
            serviceRegistry.Start(new NancyRegistryTenant(new Uri("http://localhost:9102")), new ConsulRegistryHost(consulConfiguration), 
                "customers", "v1", relativePaths: new [] { "/customers"} );

            Console.ReadLine();
        }
    }
}
