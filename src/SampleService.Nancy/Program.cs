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
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            Console.WriteLine("Press ENTER to exit");

            var serviceRegistry = new ServiceRegistry();
            serviceRegistry.Start(new NancyRegistryTenant(new Uri("http://localhost:9001")), new ConsulRegistryHost(), 
                "customers", "v1");

            Console.ReadLine();
        }
    }
}
