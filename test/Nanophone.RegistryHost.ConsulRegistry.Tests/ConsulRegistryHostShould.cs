using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Nanophone.Core;

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
        public async Task UseKeyValueStore()
        {
            const string KEY = "hello";
            DateTime dateValue = new DateTime(2016, 5, 28);

            await _registryHost.KeyValuePutAsync(KEY, dateValue);
            var value = await _registryHost.KeyValueGetAsync<DateTime>("hello");
            Assert.Equal(dateValue, value);

            await _registryHost.KeyValueDeleteAsync(KEY);
        }

        [Fact]
        public async Task UseKeyValueStoreWithFolders()
        {
            const string FOLDER = "folder/hello/world/";
            const string KEY = "date";
            DateTime dateValue = new DateTime(2016, 5, 28);

            await _registryHost.KeyValuePutAsync(FOLDER + KEY, dateValue);
            var value = await _registryHost.KeyValueGetAsync<DateTime>(FOLDER + KEY);
            Assert.Equal(dateValue, value);

            await _registryHost.KeyValueDeleteTreeAsync(FOLDER);
        }
    }
}
