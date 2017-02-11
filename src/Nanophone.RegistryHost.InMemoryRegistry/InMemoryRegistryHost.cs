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

        private readonly List<RegistryInformation> _serviceInstances = new List<RegistryInformation>();

        public KeyValues KeyValues { get; set; } = new KeyValues();

        public IList<RegistryInformation> ServiceInstances
        {
            get { return _serviceInstances; }
            set
            {
                foreach (var registryInformation in value)
                {
                    string url = registryInformation.Address;
                    if (registryInformation.Port >= 0)
                    {
                        url += $":{registryInformation.Port}";
                    }
                    RegisterServiceAsync(registryInformation.Name, registryInformation.Version, new Uri(url), tags: registryInformation.Tags);
                }
            }
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
            var instances = await FindServiceInstancesAsync().ConfigureAwait(false);
            return instances.Where(x => x.Name == name).ToList();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            var instances = await FindServiceInstancesAsync(name).ConfigureAwait(false);
            var range = new Range(version);

            return instances.Where(x => range.IsSatisfied(x.Version)).ToList();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate, Predicate<RegistryInformation> registryInformationPredicate)
        {
            return (await GetServicesCatalogAsync().ConfigureAwait(false))
                .Where(kvp => nameTagsPredicate(kvp))
                .Select(kvp => kvp.Key)
                .Select(FindServiceInstancesAsync)
                .SelectMany(task => task.Result)
                .Where(x => registryInformationPredicate(x))
                .ToList();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate)
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: predicate, registryInformationPredicate: x => true).ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<RegistryInformation> predicate)
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: x => true, registryInformationPredicate: predicate).ConfigureAwait(false);
        }

        public Task<IList<RegistryInformation>> FindAllServicesAsync()
        {
            return Task.FromResult(ServiceInstances);
        }

        public Task<RegistryInformation> RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var registryInformation = new RegistryInformation
            {
                Name = serviceName,
                Id = Guid.NewGuid().ToString(),
                Address = uri.Host,
                Port = uri.Port,
                Version = version,
                Tags = tags ?? Enumerable.Empty<string>()
            };
            ServiceInstances.Add(registryInformation);
            s_log.Info($"Registering {serviceName} service at {uri}");

            return Task.FromResult(registryInformation);
        }

        public async Task<bool> DeregisterServiceAsync(string serviceId)
        {
            var instance = (await FindServiceInstancesAsync().ConfigureAwait(false)).FirstOrDefault(x => x.Id == serviceId);
            if (instance != null)
            {
                ServiceInstances.Remove(instance);
                s_log.Info($"Deregistration of {serviceId} succeeded");

                return true;
            }

            return false;
        }

        public Task<string> RegisterHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, TimeSpan? interval = null, string notes = null)
        {
            return Task.FromResult(default(string));
        }

        public Task<bool> DeregisterHealthCheckAsync(string checkId)
        {
            return Task.FromResult(false);
        }

        public async Task KeyValuePutAsync(string key, string value)
        {
            await KeyValues.KeyValuePutAsync(key, value).ConfigureAwait(false);
        }

        public async Task<string> KeyValueGetAsync(string key)
        {
            return await KeyValues.KeyValueGetAsync(key).ConfigureAwait(false);
        }

        public async Task KeyValueDeleteAsync(string key)
        {
            await KeyValues.KeyValueDeleteAsync(key).ConfigureAwait(false);
        }

        public async Task KeyValueDeleteTreeAsync(string prefix)
        {
            await KeyValues.KeyValueDeleteTreeAsync(prefix).ConfigureAwait(false);
        }

        public async Task<string[]> KeyValuesGetKeysAsync(string prefix)
        {
            return await KeyValues.KeyValuesGetKeysAsync(prefix).ConfigureAwait(false);
        }
    }
}
