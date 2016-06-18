using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Hosting;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.WebApi;
using NLog;

namespace SampleService.WebApi.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            const bool USING_FABIO = false;

            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            Console.WriteLine("Press ENTER to exit");

            string url = "http://localhost:9000/";
            var serviceRegistry = new ServiceRegistry();
            var consulConfiguration = USING_FABIO
                ? new ConsulRegistryHostConfiguration { FabioUri = new Uri("http://my.fabio.host:1234") }
                : null;
            serviceRegistry.Start(new WebApiRegistryTenant(new Uri(url)), new ConsulRegistryHost(consulConfiguration), 
                "date", "1.7-pre", relativePaths: new [] { "/date" });

            WebApp.Start<Startup>(url);

            Console.ReadLine();
        }
    }
}
