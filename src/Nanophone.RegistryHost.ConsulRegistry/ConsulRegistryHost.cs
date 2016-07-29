using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry.Logging;
using Newtonsoft.Json;

namespace Nanophone.RegistryHost.ConsulRegistry
{
    public class ConsulRegistryHost : IRegistryHost
    {
        private const string VERSION_PREFIX = "version-";

        private static readonly ILog s_log = LogProvider.For<ConsulRegistryHost>();

        private readonly ConsulRegistryHostConfiguration _configuration;
        private readonly ConsulClient _consul;

        public ConsulRegistryHost(ConsulRegistryHostConfiguration configuration = null)
        {
            _configuration = configuration ?? new ConsulRegistryHostConfiguration { ConsulHost = "localhost", ConsulPort = 8500 };
            _consul = new ConsulClient();
        }

        private string GetVersionFromStrings(IEnumerable<string> strings)
        {
            return strings
                ?.FirstOrDefault(x => x.StartsWith(VERSION_PREFIX, StringComparison.Ordinal))
                .TrimStart(VERSION_PREFIX);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync()
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: x => true, registryInformationPredicate: x => true);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name)
        {
            var queryResult = await _consul.Health.Service(name, tag: "", passingOnly: true);
            var instances = queryResult.Response.Select(serviceEntry => new RegistryInformation
            {
                Name = serviceEntry.Service.Service,
                Address = serviceEntry.Service.Address,
                Port = serviceEntry.Service.Port,
                Version = GetVersionFromStrings(serviceEntry.Service.Tags)
            });

            return instances.ToList();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            var instances = await FindServiceInstancesAsync(name);
            return instances.Where(x => x.Version == version).ToArray();
        }

        private async Task<IDictionary<string, string[]>> GetServicesCatalogAsync()
        {
            var queryResult = await _consul.Catalog.Services(); // local agent datacenter is implied
            var services = queryResult.Response;

            return services;
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

        public async Task<IList<RegistryInformation>> FindAllServicesAsync()
        {
            var queryResult = await _consul.Agent.Services();
            var instances = queryResult.Response.Select(serviceEntry => new RegistryInformation
            {
                Name = serviceEntry.Value.Service,
                Id = serviceEntry.Value.ID,
                Address = serviceEntry.Value.Address,
                Port = serviceEntry.Value.Port,
                Version = GetVersionFromStrings(serviceEntry.Value.Tags)
                // TODO: KeyValuePairs = serviceEntry.Value.Tags.Select(prefix)
            });

            return instances.ToList();
        }

        private string GetServiceId(string serviceName, Uri uri)
        {
            return $"{serviceName}_{uri.Host.Replace(".", "_")}_{uri.Port}";
        }

        public async Task RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
        {
            var serviceId = GetServiceId(serviceName, uri);
            string check = healthCheckUri?.ToString() ?? $"{uri}".TrimEnd('/') + "/status";
            s_log.Info($"Registering {serviceName} service at {uri} on Consul {_configuration.ConsulHost}:{_configuration.ConsulPort} with status check {check}");

            string versionLabel = $"{VERSION_PREFIX}{version}";
            var keyValueTags = keyValuePairs?.Select(kvp => $"{kvp.Key}{kvp.Value}");
            var tags = new List<string>(keyValueTags ?? Enumerable.Empty<string>()) { versionLabel };

            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Tags = tags.ToArray(),
                Address = uri.Host,
                Port = uri.Port,
                Check = new AgentServiceCheck { HTTP = check, Interval = TimeSpan.FromSeconds(1) }
            };

            await _consul.Agent.ServiceRegister(registration);
            s_log.Info($"Registration of {serviceName} with id {serviceId} succeeded");
        }

        public async Task DeregisterServiceAsync(string serviceId)
        {
            var writeResult = await _consul.Agent.ServiceDeregister(serviceId);
            s_log.Info($"Deregistration of {serviceId} {(writeResult.StatusCode == System.Net.HttpStatusCode.OK ? "succeeded" : "failed")}");
        }

        public async Task KeyValuePutAsync(string key, object value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            var keyValuePair = new KVPair(key) { Value = Encoding.UTF8.GetBytes(serialized) };
            await _consul.KV.Put(keyValuePair);
        }

        public async Task<T> KeyValueGetAsync<T>(string key)
        {
            var queryResult = await _consul.KV.Get(key);
            var serialized = Encoding.UTF8.GetString(queryResult.Response.Value, 0, queryResult.Response.Value.Length);
            var result = JsonConvert.DeserializeObject<T>(serialized);

            return result;
        }

        public async Task KeyValueDeleteAsync(string key)
        {
            await _consul.KV.Delete(key);
        }

        public async Task KeyValueDeleteTreeAsync(string prefix)
        {
            await _consul.KV.DeleteTree(prefix);
        }
    }
}
