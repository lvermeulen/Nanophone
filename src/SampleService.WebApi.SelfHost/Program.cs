using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Hosting;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.WebApi;

namespace SampleService.WebApi.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:9000/";

            var serviceRegistry = new ServiceRegistry();
            serviceRegistry.Bootstrap(new WebApiRegistryTenant(new Uri(url)), new ConsulRegistryHost(), "names", "1.7-pre");

            WebApp.Start<Startup>(url);

            Console.ReadLine();
        }
    }
}
