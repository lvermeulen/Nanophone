using System;
using Nancy.Hosting.Self;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;

namespace SampleService.Nancy.Hosting.Self.Net46
{
    class Program
    {
        static void Main()
        {
            const int PORT = 8899;

            var uri = new Uri($"http://localhost:{PORT}/");
            using (var nancyHost = new NancyHost(uri))
            {
                nancyHost.Start();

                var consulRegistryHost = new ConsulRegistryHost();
                var serviceRegistry = new ServiceRegistry(consulRegistryHost);

                serviceRegistry.RegisterServiceAsync("price", "1.3.2", uri)
                    .Wait();

                Console.WriteLine($"Now listening on {uri}/price. Press enter to stop");
                Console.ReadKey();
            }
        }
    }
}
