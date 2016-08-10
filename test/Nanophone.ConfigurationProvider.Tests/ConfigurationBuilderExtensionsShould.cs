using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;
using Nanophone.RegistryHost.ConsulRegistry;

namespace Nanophone.ConfigurationProvider.Tests
{
    public class ConfigurationBuilderExtensionsShould
    {
        private readonly IRegistryHost _inMemoryRegistryHost;
        private readonly IRegistryHost _consulRegistryHost;

        public ConfigurationBuilderExtensionsShould()
        {
            _inMemoryRegistryHost = GetInMemoryRegistryHost();
            _consulRegistryHost = GetConsulRegistryHost();
        }

        private IRegistryHost GetInMemoryRegistryHost()
        {
            var registryHost = new InMemoryRegistryHost
            {
                KeyValues = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("key1", "value1"),
                    new KeyValuePair<string, object>("key2", "value2"),
                    new KeyValuePair<string, object>("folder/key3", "value3"),
                    new KeyValuePair<string, object>("folder/key4", "value4"),
                }
            };
            registryHost.KeyValueDeleteAsync("key3").Wait();

            return registryHost;
        }

        private IRegistryHost GetConsulRegistryHost()
        {
            var registryHost = new ConsulRegistryHost();
            registryHost.KeyValuePutAsync("key1", "value1").Wait();
            registryHost.KeyValuePutAsync("key2", "value2").Wait();
            registryHost.KeyValuePutAsync("folder/key3", "value3").Wait();
            registryHost.KeyValuePutAsync("folder/key4", "value4").Wait();
            registryHost.KeyValueDeleteAsync("key3").Wait();

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
