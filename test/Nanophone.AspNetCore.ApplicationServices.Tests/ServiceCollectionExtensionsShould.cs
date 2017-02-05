using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.AspNetCore.ApplicationServices.Tests
{
    public class ServiceCollectionExtensionsShould
    {
        private IRegistryHost GetRegistryHost()
        {
            return new InMemoryRegistryHost
            {
                ServiceInstances = new List<RegistryInformation>
                {
                    new RegistryInformation
                    {
                        Name = nameof(ServiceCollectionExtensionsShould),
                        Address = "http://0.0.0.0"
                    }
                }
            };
        }

        [Fact]
        public void AddNanophone()
        {
            var hostBuilder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddNanophone(GetRegistryHost);
                })
                .Configure(app =>
                {
                    var serviceRegistry = app.ApplicationServices.GetService<ServiceRegistry>();
                    Assert.NotNull(serviceRegistry);
                });

            using (new TestServer(hostBuilder))
            {
                // ConfigureServices
                // Configure
            }
        }

        [Fact]
        public void ResolveServiceInstances()
        {
            var hostBuilder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddNanophone(GetRegistryHost);
                })
                .Configure(async app =>
                {
                    var serviceRegistry = app.ApplicationServices.GetService<ServiceRegistry>();
                    Assert.NotNull(serviceRegistry);

                    var instances = await serviceRegistry.FindAllServicesAsync();
                    Assert.Equal(1, instances.Count);
                });

            using (new TestServer(hostBuilder))
            {
                // ConfigureServices
                // Configure
            }
        }
    }
}
