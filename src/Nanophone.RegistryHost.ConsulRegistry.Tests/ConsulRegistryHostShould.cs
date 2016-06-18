using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Nanophone.Core;

namespace Nanophone.RegistryHost.ConsulRegistry.Tests
{
    [TestFixture]
    public class ConsulRegistryHostShould
    {
        private IRegistryHost _registry;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            var configuration = new ConsulRegistryHostConfiguration();
            _registry = new ConsulRegistryHost(configuration);
        }

        [Test]
        public async Task FindServices()
        {
            var services = await _registry.FindServiceInstancesAsync("consul");

            Assert.IsNotNull(services);
            Assert.IsTrue(services.Any());
        }

        [Test]
        public async Task UseKeyValueStore()
        {
            const string KEY = "hello";
            DateTime dateValue = new DateTime(2016, 5, 28);

            await _registry.KeyValuePutAsync(KEY, dateValue);
            var value = await _registry.KeyValueGetAsync<DateTime>("hello");
            Assert.AreEqual(dateValue, value);

            await _registry.KeyValueDeleteAsync(KEY);
        }

        [Test]
        public async Task UseKeyValueStoreWithFolders()
        {
            const string FOLDER = "folder/hello/world/";
            const string KEY = "date";
            DateTime dateValue = new DateTime(2016, 5, 28);

            await _registry.KeyValuePutAsync(FOLDER + KEY, dateValue);
            var value = await _registry.KeyValueGetAsync<DateTime>(FOLDER + KEY);
            Assert.AreEqual(dateValue, value);

            await _registry.KeyValueDeleteTreeAsync(FOLDER);
        }
    }
}
