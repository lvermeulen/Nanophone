using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;

namespace SampleClient
{
    class Program
    {
        static void Main()
        {
            var serviceRegistry = new ServiceRegistry();
            serviceRegistry.StartClient(new ConsulRegistryHost());

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
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
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
