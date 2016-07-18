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
        private readonly List<IHandleServiceRegistration> _serviceRegistrationHandlers = new List<IHandleServiceRegistration>();


        private void StartRemovingCriticalServices()
        {
            if (_configuration.IgnoreCriticalServices)
            {
                return;
            }

            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(_configuration.CleanupDelay);
                s_log.Info("Starting to remove services in critical state");

                while (true)
                {
                    try
                    {
                        // deregister critical services
                        var queryResult = await _consul.Health.State(CheckStatus.Critical);
                        var criticalServiceIds = queryResult.Response
                            .Where(x => x.NeedsStatusCheck())
                            .Select(x => x.ServiceID);
                        foreach (var serviceId in criticalServiceIds)
                        {
                            await DeregisterServiceAsync(serviceId);
                        }
                    }
                    catch (Exception ex)
                    {
                        s_log.ErrorException("Error while removing critical services", ex);
                    }

                    await Task.Delay(_configuration.CleanupInterval);
                }
            });
        }

        public ConsulRegistryHost(ConsulRegistryHostConfiguration configuration = null)
        {
            _configuration = configuration ?? ConsulRegistryHostConfiguration.Default;
            if (_configuration.CleanupDelay == TimeSpan.MinValue)
            {
                _configuration.CleanupDelay = TimeSpan.FromSeconds(10);
            }
            if (_configuration.CleanupInterval == TimeSpan.MinValue)
            {
                _configuration.CleanupInterval = TimeSpan.FromSeconds(5);
            }

            _consul = new ConsulClient();
        }

        private string GetVersionFromStrings(IEnumerable<string> strings)
        {
            return strings
                ?.FirstOrDefault(x => x.StartsWith(VERSION_PREFIX, StringComparison.Ordinal))
                .TrimStart(VERSION_PREFIX);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name)
        {
            var queryResult = await _consul.Health.Service(name, tag: "", passingOnly: true);
            var instances = queryResult.Response
                .Select(serviceEntry =>
                {
                    string serviceAddress = serviceEntry.Service.Address;
                    int servicePort = serviceEntry.Service.Port;
                    string version = GetVersionFromStrings(serviceEntry.Service.Tags);

                    return new RegistryInformation(serviceAddress, servicePort, version);
                });

            return instances.ToList();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            var instances = await FindServiceInstancesAsync(name);
            return instances.Where(x => x.Version == version).ToArray();
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
            var tags = new List<string> { versionLabel };

            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Tags = tags.ToArray(),
                Address = uri.Host,
                Port = uri.Port,
                Check = new AgentServiceCheck { HTTP = check, Interval = TimeSpan.FromSeconds(1) }
            };

            // call registered handlers
            foreach (var serviceRegistrationHandler in _serviceRegistrationHandlers)
            {
                registration = serviceRegistrationHandler.Handle(registration, uri, keyValuePairs);
            }

            await _consul.Agent.ServiceRegister(registration);
            s_log.Info($"Registration of {serviceName} with id {serviceId} succeeded");

            StartRemovingCriticalServices();
        }

        public async Task DeregisterServiceAsync(string serviceId)
        {
            var writeResult = await _consul.Agent.ServiceDeregister(serviceId);
            s_log.Info($"Deregistration of {serviceId} {(writeResult.StatusCode == System.Net.HttpStatusCode.OK ? "succeeded" : "failed")}");
        }

        public Task StartClientAsync()
        {
            StartRemovingCriticalServices();
            return Task.FromResult(0);
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

        public void AddBeforeRegistrationHandler(IHandleServiceRegistration handler)
        {
            _serviceRegistrationHandlers.Add(handler);
        }

        public void RemoveBeforeRegistrationHandler(IHandleServiceRegistration handler)
        {
            _serviceRegistrationHandlers.Remove(handler);
        }
    }
}
