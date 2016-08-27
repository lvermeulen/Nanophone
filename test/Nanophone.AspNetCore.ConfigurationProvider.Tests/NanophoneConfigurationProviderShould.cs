using System;
using System.Globalization;
using System.Linq;
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
            _inMemoryProvider = new NanophoneConfigurationProvider(GetInMemoryRegistryHost);
            _consulProvider = new NanophoneConfigurationProvider(GetConsulRegistryHost);
        }

        private IRegistryHost GetInMemoryRegistryHost()
        {
            var registryHost = new InMemoryRegistryHost
            {
                KeyValues = new KeyValues()
                    .WithKeyValue("key1", "value1")
                    .WithKeyValue("key2", "value2")
                    .WithKeyValue("folder/key3", "value3")
                    .WithKeyValue("folder/key4", "value4")
            };

            return registryHost;
        }

        private IRegistryHost GetConsulRegistryHost()
        {
            var registryHost = new ConsulRegistryHost();
            registryHost.KeyValuePutAsync("key1", "value1").Wait();
            registryHost.KeyValuePutAsync("key2", "value2").Wait();
            registryHost.KeyValuePutAsync("folder/key3", "value3").Wait();
            registryHost.KeyValuePutAsync("folder/key4", "value4").Wait();

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
            string key = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            string expectedValue = nameof(NanophoneConfigurationProviderShould.Set);

            provider.Set(key, expectedValue);
            string value;
            Assert.True(provider.TryGet(key, out value));
            Assert.Equal(expectedValue, value);
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
