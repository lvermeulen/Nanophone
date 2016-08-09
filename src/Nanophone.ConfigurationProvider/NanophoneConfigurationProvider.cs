using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Nanophone.Core;

namespace Nanophone.ConfigurationProvider
{
    public class NanophoneConfigurationProvider : IConfigurationProvider
    {
        private readonly ConfigurationReloadToken _configurationReloadToken = new ConfigurationReloadToken();
        private readonly ServiceRegistry _serviceRegistry;

        public NanophoneConfigurationProvider(Func<IRegistryHost> registryHostFactory)
        {
            if (registryHostFactory == null)
            {
                throw new ArgumentNullException(nameof(registryHostFactory));
            }

            var registryHost = registryHostFactory();
            _serviceRegistry = new ServiceRegistry(registryHost);
        }

        public bool TryGet(string key, out string value)
        {
            value = _serviceRegistry.KeyValueGetAsync<string>(key)
                .Result;
            return true;
        }

        public void Set(string key, string value)
        {
            _serviceRegistry.KeyValuePutAsync(key, value)
                .Wait();
        }

        public IChangeToken GetReloadToken() => _configurationReloadToken;

        public void Load()
        {
            // requests are always up-to-date
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            // TODO: add Keys & List to IHaveKeyValues
            return Enumerable.Empty<string>();
        }
    }
}
