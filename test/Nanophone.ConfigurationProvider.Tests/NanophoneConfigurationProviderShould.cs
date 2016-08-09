using System.Collections.Generic;
using Nanophone.RegistryHost.InMemoryRegistry;
using Xunit;

namespace Nanophone.ConfigurationProvider.Tests
{
    public class NanophoneConfigurationProviderShould
    {
        private readonly NanophoneConfigurationProvider _provider;

        public NanophoneConfigurationProviderShould()
        {
            var registryHost = new InMemoryRegistryHost();
            registryHost.KeyValues = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("key1", "value1"),
                new KeyValuePair<string, object>("key2", "value2")
            };

            _provider = new NanophoneConfigurationProvider(() => registryHost);
        }

        [Fact]
        public void TryGet()
        {
            string value;
            Assert.True(_provider.TryGet("key1", out value));
            Assert.Equal("value1", value);

            Assert.True(_provider.TryGet("key2", out value));
            Assert.Equal("value2", value);
        }

        [Fact]
        public void Set()
        {
            _provider.Set("key3", "value3");
            string value;
            Assert.True(_provider.TryGet("key3", out value));
            Assert.Equal("value3", value);
        }

        [Fact]
        public void GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            // TODO: add Keys & List to IHaveKeyValues
            //Assert.True(false);
        }
    }
}
