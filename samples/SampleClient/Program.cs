using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.Fabio;
using Nanophone.RegistryHost.ConsulRegistry;
using NLog;

namespace SampleClient.netcore
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

            // XXX
            if (USING_FABIO)
            {
                var fabioHandler = new FabioAdapter(new Uri("http://localhost:9999"));
                serviceRegistry.ResolveServiceInstancesWith(fabioHandler);
                consulRegistryHost.AddBeforeRegistrationHandler(fabioHandler);
            }

            serviceRegistry.StartClient(consulRegistryHost);

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        string serviceName = "date";
                        var instances = serviceRegistry.FindServiceInstancesAsync(serviceName).Result;

                        Console.WriteLine($"{instances.Count} instance{(instances.Count == 1 ? "" : "s")} found");
                        foreach (var instance in instances)
                        {
                            Console.WriteLine($"    Name: {serviceName}, Address: {instance.Address}:{instance.Port}, Version: {instance.Version}");
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
