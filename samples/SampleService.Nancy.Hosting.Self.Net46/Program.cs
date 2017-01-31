using System;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nito.AsyncEx;

namespace SampleService.Nancy.Hosting.Self.Net46
{
    class Program
    {
        private static async Task MainAsync()
        {
            const int PORT = 8899;

            var uri = new Uri($"http://localhost:{PORT}/");
            using (var nancyHost = new NancyHost(uri))
            {
                nancyHost.Start();

                var consulRegistryHost = new ConsulRegistryHost();
                var serviceRegistry = new ServiceRegistry(consulRegistryHost);

                await serviceRegistry.RegisterServiceAsync("price", "1.3.2", uri);

                Console.WriteLine($"Now listening on {uri}/price. Press enter to stop");
                Console.ReadKey();
            }
        }

        static void Main()
        {
            AsyncContext.Run(async () => await MainAsync());
        }
    }
}
