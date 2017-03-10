using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Xunit;

namespace Nanophone.RegistryHost.ConsulRegistry.Tests
{
    public class ConsulRegistryHostShould
    {
        private readonly IRegistryHost _registryHost;

        public ConsulRegistryHostShould()
        {
            var configuration = new ConsulRegistryHostConfiguration();
            _registryHost = new ConsulRegistryHost(configuration);
        }

        [Fact]
        public async Task FindServiceInstancesAsync()
        {
            var services = await _registryHost.FindServiceInstancesAsync();

            Assert.NotNull(services);
            Assert.True(services.Any());
        }

        [Fact]
        public async Task FindServiceInstancesByNameAsync()
        {
            var services = await _registryHost.FindServiceInstancesAsync("consul");

            Assert.NotNull(services);
            Assert.True(services.Any());
        }

        [Fact]
        public async Task FindServiceInstancesByTagAsync()
        {
            var registryInformation = await _registryHost.RegisterServiceAsync(nameof(FindServiceInstancesByTagAsync), "1.0.0", new Uri("http://localhost"), 
                tags: new [] {nameof(ConsulRegistryHostShould)});
            var services = await _registryHost.FindServiceInstancesAsync(kvp => 
                kvp.Key == nameof(FindServiceInstancesByTagAsync) 
                && kvp.Value.Any(x => x.Equals(nameof(ConsulRegistryHostShould)))
            );

            Assert.NotNull(services);
            Assert.True(services.Any());

            await _registryHost.DeregisterServiceAsync(registryInformation.Id);
        }

        [Fact]
        public async Task FindServiceInstancesByRegistryInformationAsync()
        {
            var services = await _registryHost.FindServiceInstancesAsync(x => x.Name == "consul");

            Assert.NotNull(services);
            Assert.True(services.Any());
        }

        [Fact]
        public async Task FindServiceInstancesByVersionAsync()
        {
            var registryInformation = await _registryHost.RegisterServiceAsync(nameof(FindServiceInstancesByVersionAsync), "1.0.0", new Uri("http://localhost"));
            var services = await _registryHost.FindServiceInstancesWithVersionAsync(nameof(FindServiceInstancesByVersionAsync), "1.0.0", passingOnly: false);

            Assert.NotNull(services);
            Assert.True(services.Any());

            await _registryHost.DeregisterServiceAsync(registryInformation.Id);
        }

        [Fact]
        public async Task RegisterServiceAsync()
        {
            var serviceName = nameof(ConsulRegistryHostShould);
            var tags = new[] {"tag1", "tag2"};
            var registryInformation = await _registryHost.RegisterServiceAsync(serviceName, serviceName, new Uri("http://localhost:1234"), null, tags);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _registryHost.RegisterHealthCheckAsync(serviceName, registryInformation.Id, null, TimeSpan.FromSeconds(30)));

            string checkId = await _registryHost.RegisterHealthCheckAsync(serviceName, registryInformation.Id, new Uri("http://localhost:4321"), TimeSpan.FromSeconds(30));

            Func<string, Task<RegistryInformation>> findTenantAsync = async s => (await ((ConsulRegistryHost)_registryHost).FindAllServicesAsync())
                .FirstOrDefault(x => x.Name == s);

            var tenant = await findTenantAsync(serviceName);
            Assert.NotNull(tenant);
            Assert.Contains(tags.First(), tenant.Tags);
            Assert.Contains(tags.Last(), tenant.Tags);
            await _registryHost.DeregisterServiceAsync(tenant.Id);
            Assert.Null(await findTenantAsync(serviceName));

            bool success = await _registryHost.DeregisterHealthCheckAsync(checkId);
            Assert.True(success);
        }

        [Fact]
        public async Task UseKeyValueStoreAsync()
        {
            const string KEY = "hello";
            var dateValue = new DateTime(2016, 5, 28);

            await _registryHost.KeyValuePutAsync(KEY, dateValue.ToString(CultureInfo.InvariantCulture));
            var value = await _registryHost.KeyValueGetAsync("hello");
            Assert.Equal(dateValue, DateTime.Parse(value, CultureInfo.InvariantCulture));

            await _registryHost.KeyValueDeleteAsync(KEY);
        }

        [Fact]
        public async Task UseKeyValueStoreWithFoldersAsync()
        {
            const string FOLDER = "folder/hello/world/";
            const string KEY = "date";
            var dateValue = new DateTime(2016, 5, 28);

            await _registryHost.KeyValuePutAsync(FOLDER + KEY, dateValue.ToString(CultureInfo.InvariantCulture));
            var value = await _registryHost.KeyValueGetAsync(FOLDER + KEY);
            Assert.Equal(dateValue, DateTime.Parse(value, CultureInfo.InvariantCulture));

            await _registryHost.KeyValueDeleteTreeAsync(FOLDER);
        }
    }
}
