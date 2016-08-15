using System;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.RegistryTenant.Nancy.Tests
{
    public class NancyRegistryTenantShould
    {
        [Fact]
        public async Task BeRegistered()
        {
            var registryHost = new InMemoryRegistryHost();
            var serviceRegistry = new ServiceRegistry(registryHost);

            // add tenant
            var uri = new Uri("http://localhost:1234");
            var tenant = new NancyRegistryTenant(uri);
            string serviceName = nameof(NancyRegistryTenantShould);
            await serviceRegistry.AddTenant(tenant, serviceName, serviceName);

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
