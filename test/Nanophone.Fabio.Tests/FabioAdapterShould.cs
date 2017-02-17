using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;
using NSubstitute;
using Xunit;

namespace Nanophone.Fabio.Tests
{
    public class FabioAdapterShould
    {
        private readonly ServiceRegistry _registry;
        private readonly FabioAdapter _fabio;

        public FabioAdapterShould()
        {
            _fabio = Substitute.For<FabioAdapter>(new Uri("http://localhost:4321"), "urlprefix-");
            _registry = new ServiceRegistry(new InMemoryRegistryHost());
            _registry.ResolveServiceInstancesWith(_fabio);
        }

        [Fact]
        public async Task ResolveServiceInstancesAsync()
        {
            await _registry.FindServiceInstancesAsync();
            await _fabio.Received().FindServiceInstancesAsync();

            await _registry.FindServiceInstancesAsync(nameof(ResolveServiceInstancesAsync));
            await _fabio.Received().FindServiceInstancesAsync(nameof(ResolveServiceInstancesAsync));

            await _registry.FindServiceInstancesWithVersionAsync(nameof(ResolveServiceInstancesAsync), nameof(ResolveServiceInstancesAsync));
            await _fabio.Received().FindServiceInstancesWithVersionAsync(nameof(ResolveServiceInstancesAsync), nameof(ResolveServiceInstancesAsync));

            await _registry.FindServiceInstancesAsync(null, null);
            await _fabio.Received().FindServiceInstancesAsync(null, null);

            await _registry.FindServiceInstancesAsync((Predicate<KeyValuePair<string, string[]>>)null);
            await _fabio.Received().FindServiceInstancesAsync((Predicate<KeyValuePair<string, string[]>>)null);

            await _registry.FindServiceInstancesAsync((Predicate<RegistryInformation>)null);
            await _fabio.Received().FindServiceInstancesAsync((Predicate<RegistryInformation>)null);

            await _registry.FindAllServicesAsync();
            await _fabio.Received().FindAllServicesAsync();
        }
    }
}
