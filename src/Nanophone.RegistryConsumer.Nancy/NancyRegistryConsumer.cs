using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.Hosting.Self;
using Nanophone.Core;

namespace Nanophone.RegistryConsumer.Nancy
{
    public class NancyRegistryConsumer : IRegistryConsumer
    {
        private static HostConfiguration GetHostConfiguration()
        {
            return new HostConfiguration
            {
                UrlReservations = { CreateAutomatically = true }
            };
        }

        private static void StartHost(Uri uri, HostConfiguration hostConfiguration)
        {
            while (true)
            {
                try
                {
                    var host = new NancyHost(uri, new DefaultNancyBootstrapper(), hostConfiguration);
                    host.Start();
                }
                catch
                {
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    Console.WriteLine($"{typeof(NancyRegistryConsumer).Name}: unable to allocate port");
                }
            }
        }

        public Uri Start(string serviceName, string version)
        {
            var uri = DnsHelper.GetNewLocalUri();
            var hostConfiguration = GetHostConfiguration();
            StartHost(uri, hostConfiguration);

            return uri;
        }
    }
}
