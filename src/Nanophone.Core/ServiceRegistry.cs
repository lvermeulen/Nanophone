using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;

namespace Nanophone.Core
{
    public class ServiceRegistry
    {
        private static readonly ILog s_log = LogManager.GetLogger<ServiceRegistry>();

        private IRegistryHost _registryHost;
        private IRegistryTenant _registryTenant;

        public Task<RegistryInformation[]> FindServiceInstancesAsync(string name)
        {
            return _registryHost.FindServiceInstancesAsync(name);
        }

        public Task<RegistryInformation> FindServiceInstanceAsync(string name)
        {
            return _registryHost.FindServiceInstanceAsync(name);
        }

        public void BootstrapClient(IRegistryHost registryHost)
        {
            _registryHost = registryHost;
            _registryHost.BootstrapClientAsync().Wait();
        }

        public void Bootstrap(IRegistryTenant registryTenant, IRegistryHost registryHost, string serviceName, string version)
        {
            s_log.Info("Bootstrapping Nanophone");

            _registryTenant = registryTenant;
            var uri = _registryTenant.Uri;
            var serviceId = $"{serviceName}_{uri.ToString().Replace(".", "_")}_{uri.Port}";

            _registryHost = registryHost;
            try
            {
                _registryHost.RegisterServiceAsync(serviceName, serviceId, version, uri).Wait();
            }
            catch (Exception ex)
            {
                s_log.Error($"{registryTenant.GetType().Name}: unable to register service {serviceId}", ex);
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
    }
}
