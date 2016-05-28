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

        private IRegistryProvider _registryProvider;
        private IRegistryConsumer _registryConsumer;

        public Task<RegistryInformation[]> FindServiceInstancesAsync(string name)
        {
            return _registryProvider.FindServiceInstancesAsync(name);
        }

        public Task<RegistryInformation> FindServiceInstanceAsync(string name)
        {
            return _registryProvider.FindServiceInstanceAsync(name);
        }

        public void BootstrapClient(IRegistryProvider registryProvider)
        {
            _registryProvider = registryProvider;
            _registryProvider.BootstrapClientAsync().Wait();
        }

        public void Bootstrap(IRegistryConsumer registryConsumer, IRegistryProvider registryProvider, string serviceName, string version)
        {
            s_log.Info("Bootstrapping Nanophone");

            _registryConsumer = registryConsumer;
            var uri = _registryConsumer.Start(serviceName, version);
            var serviceId = $"{serviceName}_{DnsHelper.GetLocalIpAddressEscaped()}_{uri.Port}";

            _registryProvider = registryProvider;
            try
            {
                _registryProvider.RegisterServiceAsync(serviceName, serviceId, version, uri).Wait();
            }
            catch (Exception ex)
            {
                s_log.Error($"{registryConsumer.GetType().Name}: unable to register service {serviceId}", ex);
            }
        }

        public Task KeyValuePutAsync(string key, object value)
        {
            return _registryProvider.KeyValuePutAsync(key, value);
        }

        public Task<T> KeyValueGetAsync<T>(string key)
        {
            return _registryProvider.KeyValueGetAsync<T>(key);
        }
    }
}
