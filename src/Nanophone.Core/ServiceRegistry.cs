using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nanophone.Core.Logging;

namespace Nanophone.Core
{
    public class ServiceRegistry : IManageServiceInstances, IResolveServiceInstances, IHaveKeyValues
    {
        private static readonly ILog s_log = LogProvider.For<ServiceRegistry>();

        private readonly IRegistryHost _registryHost;
        private IResolveServiceInstances _serviceInstancesResolver;

        public ServiceRegistry(IRegistryHost registryHost)
        {
            s_log.Info("Starting Nanophone");
            _registryHost = registryHost;
        }

        public Task RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null,
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
        {
            _registryHost.RegisterServiceAsync(serviceName, version, uri, healthCheckUri, keyValuePairs)
                .Wait();

            return Task.FromResult(0);
        }

        public Task DeregisterServiceAsync(string serviceId)
        {
            _registryHost.DeregisterServiceAsync(serviceId)
                .Wait();

            return Task.FromResult(0);
        }

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

        public async Task<IList<RegistryInformation>> FindAllServicesAsync()
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindAllServicesAsync()
                : await _serviceInstancesResolver.FindAllServicesAsync();
        }

        public async Task AddTenant(IRegistryTenant registryTenant, string serviceName, string version, Uri healthCheckUri = null, IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
        {
            var uri = registryTenant.Uri;
            await RegisterServiceAsync(serviceName, version, uri, healthCheckUri, keyValuePairs);
        }

        public Task KeyValuePutAsync(string key, string value)
        {
            return _registryHost.KeyValuePutAsync(key, value);
        }

        public Task<string> KeyValueGetAsync(string key)
        {
            return _registryHost.KeyValueGetAsync(key);
        }

        public Task KeyValueDeleteAsync(string key)
        {
            return _registryHost.KeyValueDeleteAsync(key);
        }

        public Task KeyValueDeleteTreeAsync(string prefix)
        {
            return _registryHost.KeyValueDeleteTreeAsync(prefix);
        }

        public Task<string[]> KeyValuesGetKeysAsync(string prefix)
        {
            return _registryHost.KeyValuesGetKeysAsync(prefix);
        }

        public void ResolveServiceInstancesWith<T>(T serviceInstancesResolver)
            where T : IResolveServiceInstances
        {
            _serviceInstancesResolver = serviceInstancesResolver;
        }
    }
}
