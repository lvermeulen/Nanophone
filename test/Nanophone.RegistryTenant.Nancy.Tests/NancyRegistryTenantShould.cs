using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.RegistryTenant.Nancy.Tests
{
    public class NancyRegistryTenantShould
    {
        [Fact]
        public async Task BeRegisteredInHost()
        {
            var registryHost = new InMemoryRegistryHost();
            var serviceRegistry = new ServiceRegistry(registryHost);
            string serviceName = nameof(NancyRegistryTenantShould);
            serviceRegistry.AddTenant(new NancyRegistryTenant(new Uri("http://localhost:1234")), serviceName, serviceName)
                .Wait();

            Func<string, Task<string>> findTenant = async s =>
            {
                var catalog = await registryHost.GetServicesCatalogAsync();
                return catalog
                    .Where(x => x.Key == s)
                    .Select(x => x.Equals(default(KeyValuePair<string, string[]>)) ? null : x.Key)
                    .FirstOrDefault();
            };

            Assert.NotNull(findTenant(serviceName).Result);
            await registryHost.DeregisterServiceAsync(serviceName);
            Assert.Null(findTenant(serviceName).Result);
        }
    }
}
