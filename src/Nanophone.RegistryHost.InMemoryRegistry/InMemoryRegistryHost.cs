using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.InMemoryRegistry.Logging;
using Nanophone.SemVer;

namespace Nanophone.RegistryHost.InMemoryRegistry
{
    public class InMemoryRegistryHost : IRegistryHost
    {
        private static readonly ILog s_log = LogProvider.For<InMemoryRegistryHost>();

        private readonly List<RegistryInformation> _serviceInstances = new List<RegistryInformation>();
        private readonly List<HealthCheckInformation> _healthChecks = new List<HealthCheckInformation>();

        public KeyValues KeyValues { get; set; } = new KeyValues();

        public IList<RegistryInformation> ServiceInstances
        {
            private get { return _serviceInstances; }
            set
            {
                foreach (var registryInformation in value)
                {
                    string url = registryInformation.Address;
                    if (registryInformation.Port >= 0)
                    {
                        url += $":{registryInformation.Port}";
                    }
                    RegisterServiceAsync(registryInformation.Name, registryInformation.Version, new Uri(url), tags: registryInformation.Tags).Wait();
                }
            }
        }

        public IList<HealthCheckInformation> HealthChecks
        {
            private get { return _healthChecks; }
            set
            {
                foreach (var healthCheckInformation in value)
                {
                    RegisterHealthCheckAsync(healthCheckInformation.Name, healthCheckInformation.ServiceId, healthCheckInformation.Uri, healthCheckInformation.Interval, healthCheckInformation.Notes).Wait();
                }
            }
        }

        public IPerformHealthChecks HealthChecksPerformer { get; set; }

        public InMemoryRegistryHost()
        {
            HealthChecksPerformer = new DoNothingHealthChecksPerformer();
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

        private async Task<string> GetServiceId(string serviceName, Uri uri)
        {
            var ipAddress = await DnsHelper.GetIpAddressAsync();
            return $"{serviceName}_{ipAddress.Replace(".", "_")}_{uri.Port}";
        }

        public async Task<RegistryInformation> RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var serviceId = await GetServiceId(serviceName, uri);
            healthCheckUri = healthCheckUri ?? new Uri(uri, "status");
            s_log.Info($"Registering {serviceName} service at {uri} on in-memory host with status check {healthCheckUri}");

            var registryInformation = new RegistryInformation
            {
                Name = serviceName,
                Id = serviceId,
                Address = uri.Host,
                Port = uri.Port,
                Version = version,
                Tags = tags ?? Enumerable.Empty<string>()
            };
            ServiceInstances.Add(registryInformation);

            await RegisterHealthCheckAsync(serviceName, serviceId, healthCheckUri, TimeSpan.FromSeconds(1));

            s_log.Info($"Registration of {serviceName} with id {registryInformation.Id} succeeded");

            return registryInformation;
        }

        public async Task DeregisterServiceAsync(string serviceId)
        {
            var instance = (await FindServiceInstancesAsync()).FirstOrDefault(x => x.Id == serviceId);
            if (instance != null)
            {
                ServiceInstances.Remove(instance);
                s_log.Info($"Deregistration of {serviceId} succeeded");
            }
        }

        private string GetCheckId(string serviceId, Uri uri)
        {
            return $"{serviceId}_{uri.GetPath().Replace("/", "")}";
        }

        public async Task<string> RegisterHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, TimeSpan? interval = null, string notes = null)
        {
            if (checkUri == null)
            {
                throw new ArgumentNullException(nameof(checkUri));
            }

            var checkId = GetCheckId(serviceId, checkUri);
            var healthCheckInformation = new HealthCheckInformation
            {
                Id = checkId,
                Name = serviceName,
                Notes = notes,
                ServiceId = serviceId,
                Uri = checkUri,
                Interval = interval ?? TimeSpan.FromSeconds(15)
            };
            HealthChecks.Add(healthCheckInformation);
            await HealthChecksPerformer.AddHealthCheckAsync(healthCheckInformation);

            s_log.Info($"Registration of health check with id {checkId} on {serviceName} with id {serviceId} succeeded");

            return checkId;
        }

        public async Task<bool> DeregisterHealthCheckAsync(string checkId)
        {
            var healthCheckInformation = HealthChecks.FirstOrDefault(x => x.Id == checkId);
            bool isCheckFound = healthCheckInformation != null;
            if (isCheckFound)
            {
                HealthChecks.Remove(healthCheckInformation);
                await HealthChecksPerformer.RemoveHealthCheckAsync(healthCheckInformation);
                s_log.Info($"Deregistration of health check with id {checkId} succeeded");
            }

            return isCheckFound;
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
