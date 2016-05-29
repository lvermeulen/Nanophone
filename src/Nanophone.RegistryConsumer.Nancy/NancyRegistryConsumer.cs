using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Nanophone.Core;

namespace Nanophone.RegistryConsumer.Nancy
{
    public class NancyRegistryConsumer : IRegistryConsumer
    {
        private readonly int _port;
        private readonly Func<INancyBootstrapper> _bootstrapperFactory;

        public NancyRegistryConsumer(int port, Func<INancyBootstrapper> bootstrapperFactory)
        {
            _port = port;
            _bootstrapperFactory = bootstrapperFactory;
        }

        private HostConfiguration GetHostConfiguration()
        {
            return new HostConfiguration
            {
                UrlReservations = { CreateAutomatically = true }
            };
        }

        private void StartHost(Uri uri, HostConfiguration hostConfiguration)
        {
            while (true)
            {
                try
                {
                    var host = new NancyHost(uri, _bootstrapperFactory(), hostConfiguration);
                    host.Start();
                }
                catch (Exception ex)
                {
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    Console.WriteLine($"{typeof(NancyRegistryConsumer).Name}: unable to allocate port\n{ex.Message}");
                }
            }
        }

        public Uri Start(string serviceName, string version)
        {
            var uri = DnsHelper.GetNewLocalUri(_port);
            var hostConfiguration = GetHostConfiguration();
            StartHost(uri, hostConfiguration);

            return uri;
        }
    }
}
