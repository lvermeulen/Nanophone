using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;

namespace Nanophone.Fabio
{
    public class FabioAdapter : IResolveServiceInstances, 
        IHandleServiceRegistration
    {
        private readonly Uri _fabioUri;
        private readonly string _prefixName;

        public FabioAdapter(Uri fabioUri, string prefixName = "urlprefix-")
        {
            _fabioUri = fabioUri;
            _prefixName = prefixName;
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name)
        {
            return Task.FromResult<IList<RegistryInformation>>(new[] { new RegistryInformation(_fabioUri.GetSchemeAndHost(), _fabioUri.Port) });
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            return FindServiceInstancesAsync(name);
        }

        public AgentServiceRegistration Handle(AgentServiceRegistration registration, Uri uri, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if (registration != null)
            {
                // create urlprefix from uri host + relative path
                var urlPrefixes = keyValuePairs?
                    .Where(kvp => kvp.Key.Equals(_prefixName, StringComparison.OrdinalIgnoreCase))
                    .Select(kvp => $"{_prefixName}/" + new Uri(uri, kvp.Value).GetPath()) ?? Enumerable.Empty<string>();

                // copy urlprefixes to tags
                var tags = new List<string>(registration.Tags);
                tags.AddRange(urlPrefixes);
                registration.Tags = tags.ToArray();
            }

            return registration;
        }
    }
}
