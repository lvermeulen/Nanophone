using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.AspNetCore.ApplicationServices.Tests
{
    public class ApplicationBuilderExtensionsShould
    {
        [Fact]
        public void AddTenant()
        {
            var registryHost = new InMemoryRegistryHost();
            var hostBuilder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddNanophone(() => registryHost);
                })
                .Configure(app =>
                {
                    app.AddTenant(nameof(ApplicationBuilderExtensionsShould), "1.0.0", new Uri("http://localhost:1234"));

                    var serviceRegistry = app.ApplicationServices.GetService<ServiceRegistry>();
                    Assert.NotNull(serviceRegistry);

                    var instances = serviceRegistry.FindAllServicesAsync().Result;
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
