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
        public async Task FindServices()
        {
            var services = await _registryHost.FindServiceInstancesAsync("consul");

            Assert.NotNull(services);
            Assert.True(services.Any());
        }

        [Fact]
        public async Task RegisterService()
        {
            var serviceName = nameof(ConsulRegistryHostShould);
            var tags = new[] {"tag1", "tag2"};
            _registryHost.RegisterServiceAsync(serviceName, serviceName, new Uri("http://localhost:1234"), null, tags)
                .Wait();

            Func<string, Task<RegistryInformation>> findTenant = async s => (await ((ConsulRegistryHost)_registryHost).FindAllServicesAsync())
                .FirstOrDefault(x => x.Name == s);

            var tenant = findTenant(serviceName).Result;
            Assert.NotNull(tenant);
            Assert.Contains(tags.First(), tenant.Tags);
            Assert.Contains(tags.Last(), tenant.Tags);
            await _registryHost.DeregisterServiceAsync(tenant.Id);
            Assert.Null(findTenant(serviceName).Result);
        }

        [Fact]
        public async Task UseKeyValueStore()
        {
            const string KEY = "hello";
            var dateValue = new DateTime(2016, 5, 28);

            await _registryHost.KeyValuePutAsync(KEY, dateValue.ToString(CultureInfo.InvariantCulture));
            var value = await _registryHost.KeyValueGetAsync("hello");
            Assert.Equal(dateValue, DateTime.Parse(value, CultureInfo.InvariantCulture));

            await _registryHost.KeyValueDeleteAsync(KEY);
        }

        [Fact]
        public async Task UseKeyValueStoreWithFolders()
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
