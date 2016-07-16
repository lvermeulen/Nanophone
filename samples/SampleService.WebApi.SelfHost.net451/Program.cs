using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Hosting;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.WebApi;
using NLog;

namespace SampleService.WebApi.SelfHost.net451
{
    class Program
    {
        static void Main(string[] args)
        {
            const bool USING_FABIO = false;
            const bool IGNORE_CRITICAL_SERVICES = true;

            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            Console.WriteLine("Press ENTER to exit");

            string url = "http://localhost:9010/";
            var serviceRegistry = new ServiceRegistry();
            var consulConfiguration = USING_FABIO
                ? new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES, FabioUri = new Uri("http://localhost:9999") }
                : new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES };
            serviceRegistry.Start(new WebApiRegistryTenant(new Uri(url)), new ConsulRegistryHost(consulConfiguration), 
                USING_FABIO ? "date_v17pre" : "date", "1.7-pre", relativePaths: new [] { "/date" });

            WebApp.Start<Startup>(url);

            Console.ReadLine();
        }
    }
}
