using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;
using NSubstitute;
using Xunit;

namespace Nanophone.AspNetCore.ApplicationServices.Tests
{
    public class ApplicationBuilderExtensionsShould
    {
        [Fact]
        public Task AddRemoveTenantAndHealthCheck()
        {
            var registryHost = new InMemoryRegistryHost();
            var hostBuilder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddNanophone(() => registryHost);
                })
                .Configure(async app =>
                {
                    // add tenant
                    var registryInformation = app.AddTenant(nameof(ApplicationBuilderExtensionsShould), "1.0.0", new Uri("http://localhost:1234"));
                    Assert.NotNull(registryInformation);

                    var serviceRegistry = app.ApplicationServices.GetService<ServiceRegistry>();
                    Assert.NotNull(serviceRegistry);

                    var instances = await serviceRegistry.FindAllServicesAsync();
                    Assert.Equal(1, instances.Count);

                    // add health check
                    Assert.Null(app.AddHealthCheck(registryInformation, new Uri("http://localhost:4321")));

                    // remove health check
                    Assert.False(app.RemoveHealthCheck(nameof(AddRemoveTenantAndHealthCheck)));

                    // remove tenant
                    Assert.True(app.RemoveTenant(registryInformation.Id));
                    instances = await serviceRegistry.FindAllServicesAsync();
                    Assert.Equal(0, instances.Count);
                });

            using (new TestServer(hostBuilder))
            {
                // ConfigureServices
                // Configure
            }

            return Task.FromResult(0);
        }


        [Fact]
        public void ThrowWithInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ApplicationBuilderExtensions.AddTenant(null, nameof(ThrowWithInvalidArguments), "1.0.0", new Uri("http://localhost:4321")));
            Assert.Throws<ArgumentNullException>(() => ApplicationBuilderExtensions.RemoveTenant(null, nameof(ThrowWithInvalidArguments)));
            Assert.Throws<ArgumentNullException>(() => ApplicationBuilderExtensions.RemoveTenant(Substitute.For<IApplicationBuilder>(), null));
            Assert.Throws<ArgumentNullException>(() => ApplicationBuilderExtensions.AddHealthCheck(null, new RegistryInformation(), new Uri("http://localhost:1234")));
            Assert.Throws<ArgumentNullException>(() => ApplicationBuilderExtensions.AddHealthCheck(Substitute.For<IApplicationBuilder>(), null, new Uri("http://localhost:1234")));
            Assert.Throws<ArgumentNullException>(() => ApplicationBuilderExtensions.RemoveHealthCheck(null, nameof(ThrowWithInvalidArguments)));
            Assert.Throws<ArgumentNullException>(() => ApplicationBuilderExtensions.RemoveHealthCheck(Substitute.For<IApplicationBuilder>(), null));
        }
    }
}
