using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using NUnit.Framework;

namespace Nanophone.RegistryProvider.ConsulRegistry.Tests
{
    [TestFixture]
    public class ConsulRegistryProviderShould
    {
        private IRegistryProvider _registry;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            var configuration = new ConsulRegistryProviderConfiguration();
            _registry = new ConsulRegistryProvider(configuration);
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
        }
    }
}
