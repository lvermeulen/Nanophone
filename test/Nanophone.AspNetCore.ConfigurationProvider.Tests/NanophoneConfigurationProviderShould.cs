using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.AspNetCore.ConfigurationProvider.Tests
{
    public class NanophoneConfigurationProviderShould
    {
        private readonly NanophoneConfigurationProvider _inMemoryProvider;
        private readonly NanophoneConfigurationProvider _consulProvider;

        public NanophoneConfigurationProviderShould()
        {
            _inMemoryProvider = new NanophoneConfigurationProvider(() => GetInMemoryRegistryHostAsync().Result);
            _consulProvider = new NanophoneConfigurationProvider(() => GetConsulRegistryHostAsync().Result);
        }

        private async Task<IRegistryHost> GetInMemoryRegistryHostAsync()
        {
            var registryHost = new InMemoryRegistryHost();
            await registryHost.KeyValuePutAsync("key1", "value1");
            await registryHost.KeyValuePutAsync("key2", "value2");
            await registryHost.KeyValuePutAsync("folder/key3", "value3");
            await registryHost.KeyValuePutAsync("folder/key4", "value4");

            return registryHost;
        }

        private async Task<IRegistryHost> GetConsulRegistryHostAsync()
        {
            var registryHost = new ConsulRegistryHost();
            await registryHost.KeyValuePutAsync("key1", "value1");
            await registryHost.KeyValuePutAsync("key2", "value2");
            await registryHost.KeyValuePutAsync("folder/key3", "value3");
            await registryHost.KeyValuePutAsync("folder/key4", "value4");

            return registryHost;
        }

        private void TryGetWithConfigurationProvider(IConfigurationProvider provider)
        {
            Assert.True(provider.TryGet("key1", out string value));
            Assert.Equal("value1", value);

            Assert.True(provider.TryGet("key2", out value));
            Assert.Equal("value2", value);
        }

        [Fact]
        public void TryGet()
        {
            TryGetWithConfigurationProvider(_inMemoryProvider);
            TryGetWithConfigurationProvider(_consulProvider);
        }

        private void SetWithConfigurationProvider(IConfigurationProvider provider)
        {
            string key = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            string expectedValue = nameof(Set);

            provider.Set(key, expectedValue);
            Assert.True(provider.TryGet(key, out string value));
            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void Set()
        {
            SetWithConfigurationProvider(_inMemoryProvider);
            SetWithConfigurationProvider(_consulProvider);
        }

        private void GetChildKeysWithConfigurationProvider(IConfigurationProvider provider)
        {
            var result = provider.GetChildKeys(Enumerable.Empty<string>(), "folder/");
            Assert.Equal(new[] { "folder/key3", "folder/key4" }, result);
        }

        [Fact]
        public void GetChildKeys()
        {
            GetChildKeysWithConfigurationProvider(_inMemoryProvider);
            GetChildKeysWithConfigurationProvider(_consulProvider);
        }

        [Fact]
        public void ThrowWithInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new NanophoneConfigurationProvider(null));
        }
    }
}
