using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.AspNetCore.ConfigurationProvider.Tests
{
    public class ConfigurationBuilderExtensionsShould
    {
        private readonly IRegistryHost _inMemoryRegistryHost;
        private readonly IRegistryHost _consulRegistryHost;

        public ConfigurationBuilderExtensionsShould()
        {
            _inMemoryRegistryHost = GetInMemoryRegistryHost().Result;
            _consulRegistryHost = GetConsulRegistryHost().Result;
        }

        private async Task<IRegistryHost> GetInMemoryRegistryHost()
        {
            var registryHost = new InMemoryRegistryHost();
            await registryHost.KeyValuePutAsync("key1", "value1");
            await registryHost.KeyValuePutAsync("key2", "value2");
            await registryHost.KeyValuePutAsync("folder/key3", "value3");
            await registryHost.KeyValuePutAsync("folder/key4", "value4");

            return registryHost;
        }

        private async Task<IRegistryHost> GetConsulRegistryHost()
        {
            var registryHost = new ConsulRegistryHost();
            await registryHost.KeyValuePutAsync("key1", "value1");
            await registryHost.KeyValuePutAsync("key2", "value2");
            await registryHost.KeyValuePutAsync("folder/key3", "value3");
            await registryHost.KeyValuePutAsync("folder/key4", "value4");

            return registryHost;
        }

        private void MakeKeyValuesAvailable(IRegistryHost registryHost, string key, string expectedValue)
        {
            var config = new ConfigurationBuilder()
                .AddNanophoneKeyValues(() => registryHost)
                .Build();

            string value = new WebHostBuilder()
                .UseConfiguration(config)
                .GetSetting(key);

            Assert.Equal(expectedValue, value);
        }

        [Theory]
        [InlineData("key1", "value1")]
        [InlineData("key2", "value2")]
        [InlineData("folder/key3", "value3")]
        [InlineData("folder/key4", "value4")]
        public void MakeKeyValuesAvailable(string key, string expectedValue)
        {
            MakeKeyValuesAvailable(_inMemoryRegistryHost, key, expectedValue);
            MakeKeyValuesAvailable(_consulRegistryHost, key, expectedValue);
        }
    }
}
