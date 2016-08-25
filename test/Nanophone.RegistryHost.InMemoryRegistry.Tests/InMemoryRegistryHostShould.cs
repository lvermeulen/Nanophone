using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Xunit;

namespace Nanophone.RegistryHost.InMemoryRegistry.Tests
{
    public class InMemoryRegistryHostShould
    {
        private readonly List<RegistryInformation> _instances;
        private readonly KeyValues _keyValues;
        private readonly IRegistryHost _host;

        public InMemoryRegistryHostShould()
        {
            var oneDotOne = new RegistryInformation { Name = "One", Address = "1", Port = 1234, Version = "1.1.0", KeyValuePairs = (List<KeyValuePair<string, string>>)new KeyValues().WithKeyValue("key1", "value1").WithKeyValue("key2", "value2") };
            var oneDotTwo = new RegistryInformation { Name = "One", Address = "1", Port = 1235, Version = "1.2.0", KeyValuePairs = (List<KeyValuePair<string, string>>)new KeyValues().WithKeyValue("key1", "value1").WithKeyValue("key2", "value2") };
            var twoDotOne = new RegistryInformation { Name = "Two", Address = "2", Port = 1236, Version = "2.1.0", KeyValuePairs = (List<KeyValuePair<string, string>>)new KeyValues().WithKeyValue("key1", "value1").WithKeyValue("prefix", "/path") };
            var twoDotTwo = new RegistryInformation { Name = "Two", Address = "2", Port = 1237, Version = "2.2.0", KeyValuePairs = (List<KeyValuePair<string, string>>)new KeyValues().WithKeyValue("prefix", "/path").WithKeyValue("key2", "value2") };
            var threeDotOne = new RegistryInformation { Name = "Three", Address = "3", Port = 1238, Version = "3.1.0", KeyValuePairs = (List<KeyValuePair<string, string>>)new KeyValues().WithKeyValue("prefix", "/orders").WithKeyValue("key2", "value2") };
            var threeDotTwo = new RegistryInformation { Name = "Three", Address = "3", Port = 1239, Version = "3.2.0", KeyValuePairs = (List<KeyValuePair<string, string>>)new KeyValues().WithKeyValue("key1", "value1").WithKeyValue("prefix", "/customers") };
            var fourDotOne = new RegistryInformation { Name = "Four", Address = "4", Port = 1240, Version = "1.1.0" };
            var fourDotTwo = new RegistryInformation { Name = "Four", Address = "4", Port = 1241, Version = "1.2.0" };
            var fourDotThree = new RegistryInformation { Name = "Four", Address = "4", Port = 1242, Version = "2.1.0" };
            var fourDotFour = new RegistryInformation { Name = "Four", Address = "4", Port = 1243, Version = "2.2.0" };
            var fourDotFive = new RegistryInformation { Name = "Four", Address = "4", Port = 1244, Version = "3.2.0" };
            _instances = new List<RegistryInformation>
            {
                oneDotOne, oneDotTwo,
                twoDotOne, twoDotTwo,
                threeDotOne, threeDotTwo,
                fourDotOne, fourDotTwo, fourDotThree, fourDotFour, fourDotFive
            };

            _keyValues = new KeyValues()
                .WithKeyValue("1", "One")
                .WithKeyValue("1.1", "One.1")
                .WithKeyValue("2", 2.0.ToString(CultureInfo.InvariantCulture))
                .WithKeyValue("3", 3M.ToString(CultureInfo.InvariantCulture));

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
            var instances = await _host.FindServiceInstancesWithVersionAsync("Three", "3.2.0");
            Assert.Equal(1, instances.Count);
            Assert.Equal("3.2.0", instances.First().Version);
        }

        [Fact]
        public async Task FindServiceInstancesWithNameAndSemVerRange()
        {
            var instances = await _host.FindServiceInstancesWithVersionAsync("Four", ">=1.2.0 <3.2.0");
            Assert.Equal(3, instances.Count);
            Assert.Equal("1.2.0", instances.First().Version);
            Assert.Equal("2.2.0", instances.Last().Version);
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
            var instances = await _host.FindServiceInstancesAsync(x => x.Version == "2.1.0");
            Assert.Equal(2, instances.Count);
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
            await _host.DeregisterServiceAsync(first.Id);
            instances = await _host.FindServiceInstancesAsync(nameof(RegisterService));
            Assert.Equal(0, instances.Count);
        }

        [Fact]
        public async Task KeyValuePutGetDelete()
        {
            // add key/value
            await _host.KeyValuePutAsync("4", "Four");
            var value = await _host.KeyValueGetAsync("4");
            Assert.Equal("Four", value);

            // remove key/value
            await _host.KeyValueDeleteAsync("4");
            value = await _host.KeyValueGetAsync("4");
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
