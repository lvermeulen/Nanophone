using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nanophone.Core.Logging;

namespace Nanophone.Core
{
    public class ServiceRegistry : IManageServiceInstances, IManageHealthChecks, IResolveServiceInstances, IHaveKeyValues
    {
        private static readonly ILog s_log = LogProvider.For<ServiceRegistry>();

        private readonly IRegistryHost _registryHost;
        private IResolveServiceInstances _serviceInstancesResolver;

        public ServiceRegistry(IRegistryHost registryHost)
        {
            s_log.Info("Starting Nanophone");
            _registryHost = registryHost;
        }

        public async Task<RegistryInformation> RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var registryInformation = await _registryHost.RegisterServiceAsync(serviceName, version, uri, healthCheckUri, tags).ConfigureAwait(false);

            return registryInformation;
        }

        public async Task<bool> DeregisterServiceAsync(string serviceId)
        {
            return await _registryHost.DeregisterServiceAsync(serviceId).ConfigureAwait(false);
        }

        public async Task<string> RegisterHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, TimeSpan? interval = null, string notes = null)
        {
            return await _registryHost.RegisterHealthCheckAsync(serviceName, serviceId, checkUri, interval, notes).ConfigureAwait(false);
        }

        public async Task<bool> DeregisterHealthCheckAsync(string checkId)
        {
            return await _registryHost.DeregisterHealthCheckAsync(checkId).ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync()
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync().ConfigureAwait(false)
                : await _serviceInstancesResolver.FindServiceInstancesAsync().ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name, bool passingOnly = true)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(name, passingOnly).ConfigureAwait(false)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(name, passingOnly).ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version, bool passingOnly = true)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesWithVersionAsync(name, version, passingOnly).ConfigureAwait(false)
                : await _serviceInstancesResolver.FindServiceInstancesWithVersionAsync(name, version, passingOnly).ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate,
            Predicate<RegistryInformation> registryInformationPredicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(nameTagsPredicate, registryInformationPredicate).ConfigureAwait(false)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(nameTagsPredicate, registryInformationPredicate).ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(predicate).ConfigureAwait(false)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(predicate).ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<RegistryInformation> predicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(predicate).ConfigureAwait(false)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(predicate).ConfigureAwait(false);
        }

        public async Task<IList<RegistryInformation>> FindAllServicesAsync()
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindAllServicesAsync().ConfigureAwait(false)
                : await _serviceInstancesResolver.FindAllServicesAsync().ConfigureAwait(false);
        }

        public async Task<RegistryInformation> AddTenantAsync(IRegistryTenant registryTenant, string serviceName, string version, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var uri = registryTenant.Uri;
            return await RegisterServiceAsync(serviceName, version, uri, healthCheckUri, tags).ConfigureAwait(false);
        }

        public async Task<string> AddHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, TimeSpan? interval = null, string notes = null)
        {
            return await RegisterHealthCheckAsync(serviceName, serviceId, checkUri, interval, notes).ConfigureAwait(false);
        }

        public async Task KeyValuePutAsync(string key, string value)
        {
            await _registryHost.KeyValuePutAsync(key, value).ConfigureAwait(false);
        }

        public async Task<string> KeyValueGetAsync(string key)
        {
            return await _registryHost.KeyValueGetAsync(key).ConfigureAwait(false);
        }

        public async Task KeyValueDeleteAsync(string key)
        {
            await _registryHost.KeyValueDeleteAsync(key).ConfigureAwait(false);
        }

        public async Task KeyValueDeleteTreeAsync(string prefix)
        {
            await _registryHost.KeyValueDeleteTreeAsync(prefix).ConfigureAwait(false);
        }

        public async Task<string[]> KeyValuesGetKeysAsync(string prefix)
        {
            return await _registryHost.KeyValuesGetKeysAsync(prefix).ConfigureAwait(false);
        }

        public void ResolveServiceInstancesWith<T>(T serviceInstancesResolver)
            where T : IResolveServiceInstances
        {
            _serviceInstancesResolver = serviceInstancesResolver;
        }
    }
}
