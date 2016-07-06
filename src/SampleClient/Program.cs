using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using NLog;

namespace SampleClient
{
    class Program
    {
        static void Main()
        {
            const bool USING_FABIO = true;
            const bool IGNORE_CRITICAL_SERVICES = true;

            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            var serviceRegistry = new ServiceRegistry();
            var consulConfiguration = USING_FABIO
                ? new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES, FabioUri = new Uri("http://localhost:9999") }
                : new ConsulRegistryHostConfiguration { IgnoreCriticalServices = IGNORE_CRITICAL_SERVICES };
            serviceRegistry.StartClient(new ConsulRegistryHost(consulConfiguration));

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        string serviceName = USING_FABIO ? "date_v17pre" : "date";
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
