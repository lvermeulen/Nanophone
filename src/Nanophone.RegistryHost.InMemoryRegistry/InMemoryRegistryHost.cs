using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry.Logging;
using SemVer;

namespace Nanophone.RegistryHost.InMemoryRegistry
{
    public class InMemoryRegistryHost : IRegistryHost
    {
        private static readonly ILog s_log = LogProvider.For<InMemoryRegistryHost>();

        public IList<RegistryInformation> ServiceInstances { get; set; }
        public KeyValues KeyValues { get; set; }

        public InMemoryRegistryHost()
        {
            ServiceInstances = new List<RegistryInformation>();
            KeyValues = new KeyValues();
        }

        private Task<IDictionary<string, string[]>> GetServicesCatalogAsync()
        {
            IDictionary<string, string[]> results = ServiceInstances
                .GroupBy(x => x.Name, x => x.Tags)
                .ToDictionary(g => g.Key, g => g.SelectMany(x => x ?? Enumerable.Empty<string>()).ToArray());

            return Task.FromResult(results);
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesAsync()
        {
            return Task.FromResult(ServiceInstances);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name)
        {
            var instances = await FindServiceInstancesAsync();
            return instances.Where(x => x.Name == name).ToList();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            var instances = await FindServiceInstancesAsync(name);
            var range = new Range(version);
            
            return instances.Where(x => range.IsSatisfied(x.Version)).ToList();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate, Predicate<RegistryInformation> registryInformationPredicate)
        {
            return (await GetServicesCatalogAsync())
                .Where(kvp => nameTagsPredicate(kvp))
                .Select(kvp => kvp.Key)
                .Select(FindServiceInstancesAsync)
                .SelectMany(task => task.Result)
                .Where(x => registryInformationPredicate(x))
                .ToList();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate)
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: predicate, registryInformationPredicate: x => true);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<RegistryInformation> predicate)
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: x => true, registryInformationPredicate: predicate);
        }

        public Task<IList<RegistryInformation>> FindAllServicesAsync()
        {
            return Task.FromResult(ServiceInstances);
        }

        public Task RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            ServiceInstances.Add(new RegistryInformation
            {
                Name = serviceName,
                Id = Guid.NewGuid().ToString(),
                Address = uri.Host,
                Port = uri.Port,
                Version = version,
                Tags = tags ?? Enumerable.Empty<string>()
            });
            return Task.FromResult(0);
        }

        public async Task DeregisterServiceAsync(string serviceId)
        {
            var instance = (await FindServiceInstancesAsync()).FirstOrDefault(x => x.Id == serviceId);
            if (instance != null)
            {
                ServiceInstances.Remove(instance);
            }
        }

        public async Task KeyValuePutAsync(string key, string value)
        {
            await KeyValues.KeyValuePutAsync(key, value);
        }

        public async Task<string> KeyValueGetAsync(string key)
        {
            return await KeyValues.KeyValueGetAsync(key);
        }

        public async Task KeyValueDeleteAsync(string key)
        {
            await KeyValues.KeyValueDeleteAsync(key);
        }

        public async Task KeyValueDeleteTreeAsync(string prefix)
        {
            await KeyValues.KeyValueDeleteTreeAsync(prefix);
        }

        public async Task<string[]> KeyValuesGetKeysAsync(string prefix)
        {
            return await KeyValues.KeyValuesGetKeysAsync(prefix);
        }
    }
}
