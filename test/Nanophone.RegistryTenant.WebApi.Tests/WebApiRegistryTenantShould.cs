using System;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.RegistryTenant.WebApi.Tests
{
    public class WebApiRegistryTenantShould
    {
        [Fact]
        public async Task BeRegisteredAsync()
        {
            var registryHost = new InMemoryRegistryHost();
            var serviceRegistry = new ServiceRegistry(registryHost);

            // add tenant
            var uri = new Uri("http://localhost:1234");
            var tenant = new WebApiRegistryTenant(uri);
            string serviceName = nameof(WebApiRegistryTenantShould);
            await serviceRegistry.AddTenantAsync(tenant, serviceName, serviceName);

            // register
            var instance = (await registryHost.FindAllServicesAsync())
                .FirstOrDefault(x => x.Name == serviceName);
            Assert.Equal(uri.Host, instance.Address);

            // dergister
            await registryHost.DeregisterServiceAsync(instance.Id);
            instance = (await registryHost.FindAllServicesAsync())
                .FirstOrDefault(x => x.Name == serviceName);
            Assert.Null(instance);
        }
    }
}
