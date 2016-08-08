using System;
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
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            var consulRegistryHost = new ConsulRegistryHost();
            var serviceRegistry = new ServiceRegistry(consulRegistryHost);

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
