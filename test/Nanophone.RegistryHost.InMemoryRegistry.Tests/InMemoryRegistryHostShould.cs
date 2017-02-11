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
            var oneDotOne = new RegistryInformation { Name = "One", Address = "http://1.1.0.0", Port = 1234, Version = "1.1.0", Tags = new List<string> { "key1value1", "key2value2" } };
            var oneDotTwo = new RegistryInformation { Name = "One", Address = "http://1.2.0.0", Port = 1235, Version = "1.2.0", Tags = new List<string> { "key1value1", "key2value2" } };
            var twoDotOne = new RegistryInformation { Name = "Two", Address = "http://2.1.0.0", Port = 1236, Version = "2.1.0", Tags = new List<string> { "key1value1", "prefix/path" } };
            var twoDotTwo = new RegistryInformation { Name = "Two", Address = "http://2.2.0.0", Port = 1237, Version = "2.2.0", Tags = new List<string> { "prefix/path", "key2value2" } };
            var threeDotOne = new RegistryInformation { Name = "Three", Address = "http://3.1.0.0", Port = 1238, Version = "3.1.0", Tags = new List<string> { "prefix/orders", "key2value2" } };
            var threeDotTwo = new RegistryInformation { Name = "Three", Address = "http://3.2.0.0", Port = 1239, Version = "3.2.0", Tags = new List<string> { "key1value1", "prefix/customers" } };
            var fourDotOne = new RegistryInformation { Name = "Four", Address = "http://4.1.0.0", Port = 1240, Version = "1.1.0" };
            var fourDotTwo = new RegistryInformation { Name = "Four", Address = "http://4.2.0.0", Port = 1241, Version = "1.2.0" };
            var fourDotThree = new RegistryInformation { Name = "Four", Address = "http://4.3.0.0", Port = 1242, Version = "2.1.0" };
            var fourDotFour = new RegistryInformation { Name = "Four", Address = "http://4.4.0.0", Port = 1243, Version = "2.2.0" };
            var fourDotFive = new RegistryInformation { Name = "Four", Address = "http://4.5.0.0", Port = 1244, Version = "3.2.0" };
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

        [Fact]
        public async Task FindServiceInstancesAsync()
        {
            var instances = await _host.FindServiceInstancesAsync();
            Assert.Equal(_instances.Count, instances.Count);
        }

        [Fact]
        public async Task FindServiceInstancesWithNameAsync()
        {
            var instances = await _host.FindServiceInstancesAsync("Two");
            Assert.Equal(2, instances.Count);
            Assert.Equal("2.1.0.0", instances.First().Address);
            Assert.Equal("2.2.0.0", instances.Last().Address);
        }

        [Fact]
        public async Task FindServiceInstancesWithNameAndVersionAsync()
        {
            var instances = await _host.FindServiceInstancesWithVersionAsync("Three", "3.2.0");
            Assert.Equal(1, instances.Count);
            Assert.Equal("3.2.0", instances.First().Version);
        }

        [Fact]
        public async Task FindServiceInstancesWithNameAndSemVerRangeAsync()
        {
            var instances = await _host.FindServiceInstancesWithVersionAsync("Four", ">=1.2.0 <3.2.0");
            Assert.Equal(3, instances.Count);
            Assert.Equal("1.2.0", instances.First().Version);
            Assert.Equal("2.2.0", instances.Last().Version);
        }

        [Fact]
        public async Task FindServiceInstancesWithNameTagsAsync()
        {
            var instances = await _host.FindServiceInstancesAsync(kvp => kvp.Value.Any(x => x.Equals("prefix/path")));
            Assert.Equal(2, instances.Count);
        }

        [Fact]
        public async Task FindServiceInstancesWithRegistryInformationAsync()
        {
            var instances = await _host.FindServiceInstancesAsync(x => x.Version == "2.1.0");
            Assert.Equal(2, instances.Count);
        }

        [Fact]
        public async Task RegisterServiceAsync()
        {
            string serviceName = nameof(RegisterServiceAsync);
            string version = "";
            var uri = new Uri("http://host:1234/path?key=value"); 

            // add service
            await _host.RegisterServiceAsync(serviceName, version, uri);
            var instances = await _host.FindServiceInstancesAsync(nameof(RegisterServiceAsync));
            Assert.Equal(1, instances.Count);
            var first = instances.First();
            Assert.Equal(nameof(RegisterServiceAsync), first.Name);

            // remove service
            await _host.DeregisterServiceAsync(first.Id);
            instances = await _host.FindServiceInstancesAsync(nameof(RegisterServiceAsync));
            Assert.Equal(0, instances.Count);
        }

        [Fact]
        public async Task KeyValuePutGetDeleteAsync()
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
        public async Task KeyValueDeleteTreeAsync()
        {
            await _host.KeyValueDeleteTreeAsync("1");
            Assert.Equal(2, _keyValues.Count);
        }

        [Fact]
        public void ConvertKeyValuesToKeyValuePairList()
        {
            var keyValues = new KeyValues()
                .WithKeyValue("1", "One")
                .WithKeyValue("1.1", "One.1")
                .WithKeyValue("2", 2.0.ToString(CultureInfo.InvariantCulture))
                .WithKeyValue("3", 3M.ToString(CultureInfo.InvariantCulture));

            var list = (List<KeyValuePair<string, string>>)keyValues;
            Assert.NotNull(list);
            Assert.True(list.Any(x => x.Key == "1" && x.Value == "One"));
            Assert.True(list.Any(x => x.Key == "1.1" && x.Value == "One.1"));
            Assert.True(list.Any(x => x.Key == "2" && x.Value == 2.0.ToString(CultureInfo.InvariantCulture)));
            Assert.True(list.Any(x => x.Key == "3" && x.Value == 3M.ToString(CultureInfo.InvariantCulture)));
        }

        [Fact]
        public async Task RegisterHealthCheckAsync()
        {
            var result = await _host.RegisterHealthCheckAsync(nameof(RegisterHealthCheckAsync), nameof(RegisterHealthCheckAsync), new Uri("http://localhost"));
            Assert.Null(result);
        }

        [Fact]
        public async Task DeregisterHealthCheckAsync()
        {
            var result = await _host.DeregisterHealthCheckAsync(nameof(DeregisterHealthCheckAsync));
            Assert.False(result);
        }
    }
}
