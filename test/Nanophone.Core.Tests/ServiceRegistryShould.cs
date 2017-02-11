using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Nanophone.Core.Tests
{
    public class ServiceRegistryShould
    {
        private readonly IRegistryHost _host;
        private readonly ServiceRegistry _registry;

        public ServiceRegistryShould()
        {
            _host = Substitute.For<IRegistryHost>();
            _registry = new ServiceRegistry(_host);
        }

        [Fact]
        public async Task ManageServiceInstancesAsync()
        {
            await _registry.RegisterServiceAsync(nameof(ManageServiceInstancesAsync), "1.0.0", new Uri("http://localhost"));
            await _host.Received().RegisterServiceAsync(nameof(ManageServiceInstancesAsync), "1.0.0", new Uri("http://localhost"));

            await _registry.DeregisterServiceAsync(nameof(ManageServiceInstancesAsync));
            await _host.Received().DeregisterServiceAsync(nameof(ManageServiceInstancesAsync));

            var tenant = Substitute.For<IRegistryTenant>();
            tenant.Uri.Returns(new Uri("http://localhost"));
            await _registry.AddTenantAsync(tenant, nameof(ManageServiceInstancesAsync), "1.0.0");
            await _host.Received().RegisterServiceAsync(nameof(ManageServiceInstancesAsync), "1.0.0", new Uri("http://localhost"));
        }

        [Fact]
        public async Task ManageHealthChecksAsync()
        {
            await _registry.RegisterHealthCheckAsync(nameof(ManageHealthChecksAsync), nameof(ManageHealthChecksAsync), new Uri("http://localhost"));
            await _host.Received().RegisterHealthCheckAsync(nameof(ManageHealthChecksAsync), nameof(ManageHealthChecksAsync), new Uri("http://localhost"));

            await _registry.DeregisterHealthCheckAsync(nameof(ManageHealthChecksAsync));
            await _host.Received().DeregisterHealthCheckAsync(nameof(ManageHealthChecksAsync));

            await _registry.AddHealthCheckAsync(nameof(ManageHealthChecksAsync), nameof(ManageHealthChecksAsync), new Uri("http://localhost"));
            await _host.Received().RegisterHealthCheckAsync(nameof(ManageHealthChecksAsync), nameof(ManageHealthChecksAsync), new Uri("http://localhost"));
        }

        [Fact]
        public async Task ResolveServiceInstancesAsync()
        {
            await _registry.FindServiceInstancesAsync();
            await _host.Received().FindServiceInstancesAsync();

            await _registry.FindServiceInstancesAsync(nameof(ResolveServiceInstancesAsync));
            await _host.Received().FindServiceInstancesAsync(nameof(ResolveServiceInstancesAsync));

            await _registry.FindServiceInstancesWithVersionAsync(nameof(ResolveServiceInstancesAsync), nameof(ResolveServiceInstancesAsync));
            await _host.Received().FindServiceInstancesWithVersionAsync(nameof(ResolveServiceInstancesAsync), nameof(ResolveServiceInstancesAsync));

            await _registry.FindServiceInstancesAsync(null, null);
            await _host.Received().FindServiceInstancesAsync(null, null);

            await _registry.FindServiceInstancesAsync((Predicate<KeyValuePair<string, string[]>>)null);
            await _host.Received().FindServiceInstancesAsync((Predicate<KeyValuePair<string, string[]>>)null);

            await _registry.FindServiceInstancesAsync((Predicate<RegistryInformation>)null);
            await _host.Received().FindServiceInstancesAsync((Predicate<RegistryInformation>)null);

            await _registry.FindAllServicesAsync();
            await _host.Received().FindAllServicesAsync();
        }

        [Fact]
        public async Task HaveKeyValuesAsync()
        {
            await _registry.KeyValuePutAsync(nameof(HaveKeyValuesAsync), nameof(HaveKeyValuesAsync));
            await _host.Received().KeyValuePutAsync(nameof(HaveKeyValuesAsync), nameof(HaveKeyValuesAsync));

            await _registry.KeyValueGetAsync(nameof(HaveKeyValuesAsync));
            await _host.Received().KeyValueGetAsync(nameof(HaveKeyValuesAsync));

            await _registry.KeyValueDeleteAsync(nameof(HaveKeyValuesAsync));
            await _host.Received().KeyValueDeleteAsync(nameof(HaveKeyValuesAsync));

            await _registry.KeyValueDeleteTreeAsync(nameof(HaveKeyValuesAsync));
            await _host.Received().KeyValueDeleteTreeAsync(nameof(HaveKeyValuesAsync));

            await _registry.KeyValuesGetKeysAsync(nameof(HaveKeyValuesAsync));
            await _host.Received().KeyValuesGetKeysAsync(nameof(HaveKeyValuesAsync));
        }
    }
}
