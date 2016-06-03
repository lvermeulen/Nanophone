using System;
using System.Collections.Generic;
using System.Linq;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.Nancy;

namespace SampleService.Nancy
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Press ENTER to exit");

            var serviceRegistry = new ServiceRegistry();
            serviceRegistry.Start(new NancyRegistryTenant(new Uri("http://localhost:9001")), 
                new ConsulRegistryHost(), "customers", "v1");

            Console.ReadLine();
        }
    }
}
