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
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            Console.WriteLine("Press ENTER to exit");

            string url = "http://localhost:9000/";
            var serviceRegistry = new ServiceRegistry();
            serviceRegistry.Start(new WebApiRegistryTenant(new Uri(url)), new ConsulRegistryHost(), 
                "date", "1.7-pre");

            WebApp.Start<Startup>(url);

            Console.ReadLine();
        }
    }
}
