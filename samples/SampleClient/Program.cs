using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.Fabio;
using Nanophone.RegistryHost.ConsulRegistry;
using NLog;

namespace SampleClient
{
    class Program
    {
        static void Main()
        {
            const bool USING_FABIO = false;
            const bool IGNORE_CRITICAL_SERVICES = true;

            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            var serviceRegistry = new ServiceRegistry();
            var consulConfiguration = new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES };
            var consulRegistryHost = new ConsulRegistryHost(consulConfiguration);

            if (USING_FABIO)
            {
                var fabioHandler = new FabioAdapter(new Uri("http://localhost:9999"));
                serviceRegistry.ResolveServiceInstancesWith(fabioHandler);
                //consulRegistryHost.AddBeforeRegistrationHandler(fabioHandler);
            }

            serviceRegistry.StartClient(consulRegistryHost);

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        var instances = serviceRegistry.FindServiceInstancesAsync().Result;
                        var groupedByName = instances
                            .GroupBy(x => x.Name, x => x)
                            .ToList();

                        if (!groupedByName.Any())
                        {
                            Console.WriteLine("No service instances found");
                            Console.WriteLine();
                            continue;
                        }

                        foreach (var group in groupedByName)
                        {
                            string name = group.Key;
                            int count = group.Count();
                            Console.WriteLine($"    Name: {name}, instances found: {count}");
                        }
                        Console.WriteLine();

                        Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine($"Could not connect to service registry: {ex.Message}");
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
