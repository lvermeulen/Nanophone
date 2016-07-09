using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Nanophone.Core;

namespace Nanophone.RegistryHost.ConsulRegistry.Tests
{
    public class ConsulRegistryHostShould
    {
        private readonly IRegistryHost _registry;

        public ConsulRegistryHostShould()
        {
            var configuration = new ConsulRegistryHostConfiguration();
            _registry = new ConsulRegistryHost(configuration);
        }

        [Fact]
        public async Task FindServices()
        {
            var services = await _registry.FindServiceInstancesAsync("consul");

            Assert.NotNull(services);
            Assert.True(services.Any());
        }

        [Fact]
        public async Task UseKeyValueStore()
        {
            const string KEY = "hello";
            DateTime dateValue = new DateTime(2016, 5, 28);

            await _registry.KeyValuePutAsync(KEY, dateValue);
            var value = await _registry.KeyValueGetAsync<DateTime>("hello");
            Assert.Equal(dateValue, value);

            await _registry.KeyValueDeleteAsync(KEY);
        }

        [Fact]
        public async Task UseKeyValueStoreWithFolders()
        {
            const string FOLDER = "folder/hello/world/";
            const string KEY = "date";
            DateTime dateValue = new DateTime(2016, 5, 28);

            await _registry.KeyValuePutAsync(FOLDER + KEY, dateValue);
            var value = await _registry.KeyValueGetAsync<DateTime>(FOLDER + KEY);
            Assert.Equal(dateValue, value);

            await _registry.KeyValueDeleteTreeAsync(FOLDER);
        }
    }
}
