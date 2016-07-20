using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Xunit;

namespace Nanophone.RegistryHost.InMemoryRegistry.Tests
{
    public class InMemoryRegistryHostShould
    {
        private readonly List<RegistryInformation> _instances;
        private readonly List<KeyValuePair<string, object>> _keyValues;
        private readonly IRegistryHost _host;

        public InMemoryRegistryHostShould()
        {
            var oneDotOne = new RegistryInformation("One", "1", 1234, "1.1", KeyValues(new[] { "key1", "value1", "key2", "value2" }));
            var oneDotTwo = new RegistryInformation("One", "1", 1235, "1.2", KeyValues(new[] { "key1", "value1", "key2", "value2" }));
            var twoDotOne = new RegistryInformation("Two", "2", 1236, "2.1", KeyValues(new[] { "key1", "value1", "prefix", "/path" }));
            var twoDotTwo = new RegistryInformation("Two", "2", 1237, "2.2", KeyValues(new[] { "prefix", "/path", "key2", "value2" }));
            var threeDotOne = new RegistryInformation("Three", "3", 1238, "3.1", KeyValues(new[] { "prefix", "/orders", "key2", "value2" }));
            var threeDotTwo = new RegistryInformation("Three", "3", 1239, "3.2", KeyValues(new[] { "key1", "value1", "prefix", "/customers" }));
            _instances = new List<RegistryInformation>
            {
                oneDotOne, oneDotTwo,
                twoDotOne, twoDotTwo,
                threeDotOne, threeDotTwo
            };

            _keyValues = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("1", "One"),
                new KeyValuePair<string, object>("1.1", "One.1"),
                new KeyValuePair<string, object>("2", 2.0),
                new KeyValuePair<string, object>("3", 3M)
            };

            _host = new InMemoryRegistryHost
            {
                ServiceInstances = _instances,
                KeyValues = _keyValues
            };
        }

        private IEnumerable<KeyValuePair<string, string>> KeyValues(string[] keyValues)
        {
            // must have > 0 and even number
            if (keyValues.Length == 0 || keyValues.Length % 2 != 0)
            {
                return null;
            }

            var result = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < keyValues.Length; i++)
            {
                result.Add(new KeyValuePair<string, string>(keyValues[i], keyValues[i + 1]));
                i++;
            }

            return result;
        }

        [Fact]
        public async Task FindServiceInstances()
        {
            var instances = await _host.FindServiceInstancesAsync();
            Assert.Equal<RegistryInformation>(_instances, instances);
        }

        [Fact]
        public async Task FindServiceInstancesWithName()
        {
            var instances = await _host.FindServiceInstancesAsync("Two");
            Assert.Equal(2, instances.Count);
            Assert.Equal("2", instances.First().Address);
        }

        [Fact]
        public async Task FindServiceInstancesWithNameAndVersion()
        {
            var instances = await _host.FindServiceInstancesWithVersionAsync("Three", "3.2");
            Assert.Equal(1, instances.Count);
            Assert.Equal("3.2", instances.First().Version);
        }

        [Fact]
        public async Task FindServiceInstancesWithNameTags()
        {
            var instances = await _host.FindServiceInstancesAsync(kvp => kvp.Value.Any(x => x.Equals("prefix/path")));
            Assert.Equal(2, instances.Count);
        }

        [Fact]
        public async Task FindServiceInstancesWithRegistryInformation()
        {
            var instances = await _host.FindServiceInstancesAsync(x => x.Version == "2.1");
            Assert.Equal(1, instances.Count);
        }

        [Fact]
        public async Task RegisterService()
        {
            string serviceName = nameof(RegisterService);
            string version = "";
            var uri = new Uri("http://host:1234/path?key=value"); 

            // add service
            await _host.RegisterServiceAsync(serviceName, version, uri);
            var instances = await _host.FindServiceInstancesAsync(nameof(RegisterService));
            Assert.Equal(1, instances.Count);
            var first = instances.First();
            Assert.Equal(nameof(RegisterService), first.Name);

            // remove service
            _instances.Remove(first);
            instances = await _host.FindServiceInstancesAsync(nameof(RegisterService));
            Assert.Equal(0, instances.Count);
        }

        [Fact]
        public async Task KeyValuePutGetDelete()
        {
            // add key/value
            await _host.KeyValuePutAsync("4", "Four");
            var value = await _host.KeyValueGetAsync<string>("4");
            Assert.Equal("Four", value);

            // remove key/value
            await _host.KeyValueDeleteAsync("4");
            value = await _host.KeyValueGetAsync<string>("4");
            Assert.Equal(null, value);
        }

        [Fact]
        public async Task KeyValueDeleteTree()
        {
            await _host.KeyValueDeleteTreeAsync("1");
            Assert.Equal(2, _keyValues.Count);
        }
    }
}
