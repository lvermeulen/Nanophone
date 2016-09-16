using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nanophone.AspNetCore.ConfigurationProvider;
using Nanophone.RegistryHost.ConsulRegistry;
using NLog;

namespace SampleService.AspNetCore.Kestrel
{
    public class Program
    {
        public const int PORT = 9030;

        public static void Main()
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Debug($"Starting {typeof(Program).Namespace}");

            var config = new ConfigurationBuilder()
                .AddNanophoneKeyValues(() => new ConsulRegistryHost())
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{PORT}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
