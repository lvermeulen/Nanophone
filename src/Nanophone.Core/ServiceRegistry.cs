using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nanophone.Core.Logging;

namespace Nanophone.Core
{
    public class ServiceRegistry
    {
        private static readonly ILog s_log = LogProvider.For<ServiceRegistry>();

        private IRegistryHost _registryHost;
        private IRegistryTenant _registryTenant;

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name)
        {
            return await _registryHost.FindServiceInstancesAsync(name);
        }

        public async Task<RegistryInformation> FindServiceInstanceAsync(string name)
        {
            return await _registryHost.FindServiceInstanceAsync(name);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            return await _registryHost.FindServiceInstancesWithVersionAsync(name, version);
        }

        public async Task<RegistryInformation> FindServiceInstanceWithVersionAsync(string name, string version)
        {
            return await _registryHost.FindServiceInstanceWithVersionAsync(name, version);
        }

        public void StartClient(IRegistryHost registryHost)
        {
            _registryHost = registryHost;
            _registryHost.StartClientAsync()
                .Wait();
        }

        public void Start(IRegistryTenant registryTenant, IRegistryHost registryHost, string serviceName, string version, Uri healthCheckUri = null, IEnumerable<string> relativePaths = null)
        {
            s_log.Info("Starting Nanophone");

            _registryTenant = registryTenant;
            var uri = _registryTenant.Uri;

            _registryHost = registryHost;
            try
            {
                _registryHost.RegisterServiceAsync(serviceName, version, uri, healthCheckUri, relativePaths)
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
    }
}
