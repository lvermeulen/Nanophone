using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            var serviceRegistry = new ServiceRegistry();
            serviceRegistry.StartClient(new ConsulRegistryHost());

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        var instances = serviceRegistry.FindServiceInstancesAsync("date").Result;

                        Console.WriteLine($"{instances.Count} instances found:");
                        foreach (var instance in instances)
                        {
                            Console.WriteLine($"    Address: {instance.Address}:{instance.Port}, Version: {instance.Version}");
                        }
                        Console.WriteLine();

                        Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine("Could not connect to service registry");
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
