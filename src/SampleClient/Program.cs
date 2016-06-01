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
        private static readonly ServiceRegistry s_serviceRegistry = new ServiceRegistry();

        private static RegistryInformation FindInstance(string serviceName)
        {
            try
            {
                return s_serviceRegistry.FindServiceInstanceAsync(serviceName).Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void CheckService(string serviceName)
        {
            var instance = FindInstance(serviceName);
            Console.WriteLine(instance != null
                ? $"Instance of \"{serviceName}\" found at {instance.Address}:{instance.Port} with version {instance.Version}"
                : $"No instance of \"{serviceName}\" found");
        }

        static void Main()
        {
            s_serviceRegistry.BootstrapClient(new ConsulRegistryHost());

            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    CheckService("customers");
                    CheckService("names");

                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
