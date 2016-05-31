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
        private readonly Func<INancyBootstrapper> _bootstrapperFactory;

        public NancyRegistryConsumer(Func<INancyBootstrapper> bootstrapperFactory)
        {
            _bootstrapperFactory = bootstrapperFactory;
        }

        private void StartHost(Uri uri)
        {
            while (true)
            {
                try
                {
                    var hostConfiguration = new HostConfiguration
                    {
                        RewriteLocalhost = true,
                        UrlReservations = { CreateAutomatically = true }
                    };

                    var host = new NancyHost(uri, _bootstrapperFactory(), hostConfiguration);
                    host.Start();
                }
                catch (Exception ex)
                {
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    Console.WriteLine($"{typeof(NancyRegistryConsumer).Name}: unable to allocate port{Environment.NewLine}{ex.Message}");
                }
            }
        }

        public Uri Start(string serviceName, string version)
        {
            var uri = DnsHelper.GetNewLocalUri();
            StartHost(uri);

            return uri;
        }
    }
}
