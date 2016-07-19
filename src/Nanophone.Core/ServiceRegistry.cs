using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core.Logging;

namespace Nanophone.Core
{
    public class ServiceRegistry : IResolveServiceInstances
    {
        private static readonly ILog s_log = LogProvider.For<ServiceRegistry>();

        private IRegistryHost _registryHost;
        private IRegistryTenant _registryTenant;
        private IResolveServiceInstances _serviceInstancesResolver;

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync()
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync()
                : await _serviceInstancesResolver.FindServiceInstancesAsync();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(name)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(name);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesWithVersionAsync(name, version)
                : await _serviceInstancesResolver.FindServiceInstancesWithVersionAsync(name, version);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate,
            Predicate<RegistryInformation> registryInformationPredicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(nameTagsPredicate, registryInformationPredicate)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(nameTagsPredicate, registryInformationPredicate);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(predicate)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(predicate);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<RegistryInformation> predicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(predicate)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(predicate);
        }

        public void StartClient(IRegistryHost registryHost)
        {
            _registryHost = registryHost;
            _registryHost.StartClientAsync()
                .Wait();
        }

        public void Start(IRegistryTenant registryTenant, IRegistryHost registryHost, string serviceName, string version, Uri healthCheckUri = null, IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
        {
            s_log.Info("Starting Nanophone");

            _registryTenant = registryTenant;
            var uri = _registryTenant.Uri;

            _registryHost = registryHost;
            try
            {
                _registryHost.RegisterServiceAsync(serviceName, version, uri, healthCheckUri, keyValuePairs)
                    .Wait();
            }
            catch (Exception ex)
            {
                s_log.ErrorException($"{registryTenant.GetType().Name}: unable to register service {serviceName}", ex);
            }
        }

        public Task KeyValuePutAsync(string key, object value)
        {
            return _registryHost.KeyValuePutAsync(key, value);
        }

        public Task<T> KeyValueGetAsync<T>(string key)
        {
            return _registryHost.KeyValueGetAsync<T>(key);
        }

        public Task KeyValueDeleteAsync(string key)
        {
            return _registryHost.KeyValueDeleteAsync(key);
        }

        public Task KeyValueDeleteTreeAsync(string prefix)
        {
            return _registryHost.KeyValueDeleteTreeAsync(prefix);
        }

        public void ResolveServiceInstancesWith<T>(T serviceInstancesResolver)
            where T : IResolveServiceInstances
        {
            _serviceInstancesResolver = serviceInstancesResolver;
        }
    }
}
