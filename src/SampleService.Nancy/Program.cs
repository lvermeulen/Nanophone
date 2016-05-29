using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nanophone.Core;
using Nanophone.RegistryConsumer.Nancy;
using Nanophone.RegistryProvider.ConsulRegistry;

namespace SampleService.Nancy
{
    class Program
    {
        private static ServiceRegistry s_serviceRegistry = new ServiceRegistry();

        static void Main()
        {
            s_serviceRegistry.Bootstrap(new NancyRegistryConsumer(8081, () => new DefaultNancyBootstrapper()), new ConsulRegistryProvider(), "customers", "v1");
            Console.ReadLine();
        }
    }
}
