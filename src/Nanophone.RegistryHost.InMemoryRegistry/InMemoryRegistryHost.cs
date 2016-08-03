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
        public IList<KeyValuePair<string, object>> KeyValues { get; set; }

        public InMemoryRegistryHost()
        {
            ServiceInstances = new List<RegistryInformation>();
            KeyValues = new List<KeyValuePair<string, object>>();
        }

        private Task<IDictionary<string, string[]>> GetServicesCatalogAsync()
        {
            IDictionary<string, string[]> results = ServiceInstances
                .GroupBy(x => x.Name, x => x.KeyValuePairs.Select(kvp => kvp.Key + kvp.Value))
                .ToDictionary(g => g.Key, g => g.SelectMany(x => x).ToArray());

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

        public Task RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null,
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
        {
            ServiceInstances.Add(new RegistryInformation
            {
                Name = serviceName,
                Id = Guid.NewGuid().ToString(),
                Address = uri.Host,
                Port = uri.Port,
                Version = version,
                KeyValuePairs = keyValuePairs ?? Enumerable.Empty<KeyValuePair<string, string>>()
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

        public Task KeyValuePutAsync(string key, object value)
        {
            KeyValues.Add(new KeyValuePair<string, object>(key, value));
            return Task.FromResult(0);
        }

        public Task<T> KeyValueGetAsync<T>(string key)
        {
            var result = KeyValues.FirstOrDefault(x => x.Key == key);
            var value = result.Equals(default(KeyValuePair<string, object>))
                ? null
                : result.Value;
            return Task.FromResult((T)value);
        }

        public Task KeyValueDeleteAsync(string key)
        {
            var deletes = KeyValues.Where(x => x.Key == key).ToArray();
            for (int i = deletes.Length-1; i >= 0; i--)
            {
                KeyValues.Remove(deletes[i]);
            }
            return Task.FromResult(0);
        }

        public Task KeyValueDeleteTreeAsync(string prefix)
        {
            var deletes = KeyValues.Where(x => x.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToArray();
            for (int i = deletes.Length - 1; i >= 0; i--)
            {
                KeyValues.Remove(deletes[i]);
            }
            return Task.FromResult(0);
        }
    }
}
