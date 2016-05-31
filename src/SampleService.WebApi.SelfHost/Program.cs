using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Nanophone.Core;
using Nanophone.RegistryConsumer.WebApi;
using Nanophone.RegistryProvider.ConsulRegistry;

namespace SampleService.WebApi.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:9000/";

            var serviceRegistry = new ServiceRegistry();
            serviceRegistry.Bootstrap(new WebApiRegistryConsumer(new Uri(url)), new ConsulRegistryProvider(), "names", "1.7-pre");

            WebApp.Start<Startup>(url);

            Console.ReadLine();
        }
    }
}
