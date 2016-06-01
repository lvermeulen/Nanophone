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
        private static ServiceRegistry s_serviceRegistry = new ServiceRegistry();

        static void Main()
        {
            s_serviceRegistry.Bootstrap(new NancyRegistryTenant(new Uri("http://localhost:9001")), new ConsulRegistryHost(), "customers", "v1");
            Console.ReadLine();
        }
    }
}
