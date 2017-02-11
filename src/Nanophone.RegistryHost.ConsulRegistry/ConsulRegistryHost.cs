using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry.Logging;
using SemVer;

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
            string consulHost = configuration?.HostName ?? "localhost";
            int consulPort = configuration?.Port ?? 8500;
            _configuration = new ConsulRegistryHostConfiguration {HostName = consulHost, Port = consulPort};

            _consul = new ConsulClient(config =>
            {
                config.Address = new Uri($"http://{_configuration.HostName}:{_configuration.Port}");
            });
        }

        private string GetVersionFromStrings(IEnumerable<string> strings)
        {
            return strings
                ?.FirstOrDefault(x => x.StartsWith(VERSION_PREFIX, StringComparison.Ordinal))
                .TrimStart(VERSION_PREFIX);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync()
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: x => true, registryInformationPredicate: x => true).ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name, bool passingOnly = true)
        {
            return await FindServiceInstancesInternalAsync(name, passingOnly);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version, bool passingOnly = true)
        {
            var instances = await FindServiceInstancesAsync(name, passingOnly).ConfigureAwait(false);
            var range = new Range(version);

            return instances.Where(x => range.IsSatisfied(x.Version)).ToArray();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesInternalAsync(string name, bool passingOnly)
        {
            var queryResult = await _consul.Health.Service(name, tag: "", passingOnly: passingOnly).ConfigureAwait(false);
            var instances = queryResult.Response.Select(serviceEntry => new RegistryInformation
            {
                Name = serviceEntry.Service.Service,
                Id = serviceEntry.Service.ID,
                Address = serviceEntry.Service.Address,
                Port = serviceEntry.Service.Port,
                Version = GetVersionFromStrings(serviceEntry.Service.Tags),
                Tags = serviceEntry.Service.Tags
            });

            return instances.ToList();
        }

        private async Task<IDictionary<string, string[]>> GetServicesCatalogAsync()
        {
            var queryResult = await _consul.Catalog.Services().ConfigureAwait(false); // local agent datacenter is implied
            var services = queryResult.Response;

            return services;
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate, Predicate<RegistryInformation> registryInformationPredicate)
        {
            return (await GetServicesCatalogAsync().ConfigureAwait(false))
                .Where(kvp => nameTagsPredicate(kvp))
                .Select(kvp => kvp.Key)
                .Select(x => FindServiceInstancesInternalAsync(x, passingOnly: false))
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

        public async Task<IList<RegistryInformation>> FindAllServicesAsync()
        {
            var queryResult = await _consul.Agent.Services().ConfigureAwait(false);
            var instances = queryResult.Response.Select(serviceEntry => new RegistryInformation
            {
                Name = serviceEntry.Value.Service,
                Id = serviceEntry.Value.ID,
                Address = serviceEntry.Value.Address,
                Port = serviceEntry.Value.Port,
                Version = GetVersionFromStrings(serviceEntry.Value.Tags),
                Tags = serviceEntry.Value.Tags
            });

            return instances.ToList();
        }

        private async Task<string> GetServiceIdAsync(string serviceName, Uri uri)
        {
            var ipAddress = await DnsHelper.GetIpAddressAsync().ConfigureAwait(false);
            return $"{serviceName}_{ipAddress.Replace(".", "_")}_{uri.Port}";
        }

        public async Task<RegistryInformation> RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var serviceId = await GetServiceIdAsync(serviceName, uri).ConfigureAwait(false);
            string check = healthCheckUri?.ToString() ?? $"{uri}".TrimEnd('/') + "/status";
            s_log.Info($"Registering {serviceName} service at {uri} on Consul {_configuration.HostName}:{_configuration.Port} with status check {check}");

            string versionLabel = $"{VERSION_PREFIX}{version}";
            var tagList = (tags ?? Enumerable.Empty<string>()).ToList();
            tagList.Add(versionLabel);

            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Tags = tagList.ToArray(),
                Address = uri.Host,
                Port = uri.Port,
                Check = new AgentServiceCheck { HTTP = check, Interval = TimeSpan.FromSeconds(1) }
            };

            await _consul.Agent.ServiceRegister(registration).ConfigureAwait(false);
            s_log.Info($"Registration of {serviceName} with id {serviceId} succeeded");

            return new RegistryInformation
            {
                Name = registration.Name,
                Id = registration.ID,
                Address = registration.Address,
                Port = registration.Port,
                Version = version,
                Tags = tagList
            };
        }

        public async Task<bool> DeregisterServiceAsync(string serviceId)
        {
            var writeResult = await _consul.Agent.ServiceDeregister(serviceId).ConfigureAwait(false);
            bool isSuccess = writeResult.StatusCode == HttpStatusCode.OK;
            string success = isSuccess ? "succeeded" : "failed";
            s_log.Info($"Deregistration of {serviceId} " + success);

            return isSuccess;
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
            var checkRegistration = new AgentCheckRegistration
            {
                ID = checkId,
                Name = serviceName,
                Notes = notes,
                ServiceID = serviceId,
                HTTP = checkUri.ToString(),
                Interval = interval
            };
            var writeResult = await _consul.Agent.CheckRegister(checkRegistration).ConfigureAwait(false);
            bool isSuccess = writeResult.StatusCode == HttpStatusCode.OK;
            string success = isSuccess ? "succeeded" : "failed";
            s_log.Info($"Registration of health check with id {checkId} on {serviceName} with id {serviceId}" + success);

            return checkId;
        }

        public async Task<bool> DeregisterHealthCheckAsync(string checkId)
        {
            var writeResult = await _consul.Agent.CheckDeregister(checkId).ConfigureAwait(false);
            bool isSuccess = writeResult.StatusCode == HttpStatusCode.OK;
            string success = isSuccess ? "succeeded" : "failed";
            s_log.Info($"Deregistration of health check with id {checkId} " + success);

            return isSuccess;
        }

        public async Task KeyValuePutAsync(string key, string value)
        {
            var keyValuePair = new KVPair(key) { Value = Encoding.UTF8.GetBytes(value) };
            await _consul.KV.Put(keyValuePair).ConfigureAwait(false);
        }

        public async Task<string> KeyValueGetAsync(string key)
        {
            var queryResult = await _consul.KV.Get(key).ConfigureAwait(false);
            if (queryResult.Response == null)
            {
                return null;
            }

            return Encoding.UTF8.GetString(queryResult.Response.Value);
        }

        public async Task KeyValueDeleteAsync(string key)
        {
            await _consul.KV.Delete(key).ConfigureAwait(false);
        }

        public async Task KeyValueDeleteTreeAsync(string prefix)
        {
            await _consul.KV.DeleteTree(prefix).ConfigureAwait(false);
        }

        public async Task<string[]> KeyValuesGetKeysAsync(string prefix)
        {
            var queryResult = await _consul.KV.Keys(prefix ?? "").ConfigureAwait(false);
            return queryResult.Response;
        }
    }
}
