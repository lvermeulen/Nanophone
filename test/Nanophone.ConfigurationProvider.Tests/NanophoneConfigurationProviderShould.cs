using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.ConfigurationProvider.Tests
{
    public class NanophoneConfigurationProviderShould
    {
        private readonly NanophoneConfigurationProvider _inMemoryProvider;
        private readonly NanophoneConfigurationProvider _consulProvider;

        public NanophoneConfigurationProviderShould()
        {
            _inMemoryProvider = new NanophoneConfigurationProvider(GetInMemoryRegistryHost);
            _consulProvider = new NanophoneConfigurationProvider(GetConsulRegistryHost);
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

        private void TryGet(IConfigurationProvider provider)
        {
            string value;
            Assert.True(provider.TryGet("key1", out value));
            Assert.Equal("value1", value);

            Assert.True(provider.TryGet("key2", out value));
            Assert.Equal("value2", value);
        }

        [Fact]
        public void TryGet()
        {
            TryGet(_inMemoryProvider);
            TryGet(_consulProvider);
        }

        private void Set(IConfigurationProvider provider)
        {
            provider.Set("key3", "value3");
            string value;
            Assert.True(provider.TryGet("key3", out value));
            Assert.Equal("value3", value);
        }

        [Fact]
        public void Set()
        {
            Set(_inMemoryProvider);
            Set(_consulProvider);
        }

        private void GetChildKeys(IConfigurationProvider provider)
        {
            var result = provider.GetChildKeys(Enumerable.Empty<string>(), "folder/");
            Assert.Equal(new[] { "folder/key3", "folder/key4" }, result);
        }

        [Fact]
        public void GetChildKeys()
        {
            GetChildKeys(_inMemoryProvider);
            GetChildKeys(_consulProvider);
        }
    }
}
