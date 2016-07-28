using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.Fabio.Logging;

namespace Nanophone.Fabio
{
    public class FabioAdapter : IResolveServiceInstances
    {
        private static readonly ILog s_log = LogProvider.For<FabioAdapter>();

        private readonly Uri _fabioUri;

        public FabioAdapter(Uri fabioUri, string prefixName = "urlprefix-")
        {
            s_log.Info($"Starting fabio adapter with address {fabioUri}, prefix {prefixName}");
            _fabioUri = fabioUri;
        }

        private Task<IList<RegistryInformation>> GetFabioResult(string name = "")
        {
            return Task.FromResult<IList<RegistryInformation>>(new[]
            {
                new RegistryInformation
                {
                    Name = name,
                    Address = _fabioUri.GetSchemeAndHost(),
                    Port = _fabioUri.Port
                }
            });
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesAsync()
        {
            s_log.Info("Finding service instances with fabio adapter");
            return GetFabioResult();
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name)
        {
            s_log.Info($"Finding service instances with fabio adapter: service {name}");
            return GetFabioResult(name);
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            s_log.Info($"Finding service instances with fabio adapter: service {name}, version {version}");
            return GetFabioResult(name);
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate, Predicate<RegistryInformation> registryInformationPredicate)
        {
            s_log.Info("Finding service instances with fabio adapter");
            return GetFabioResult();
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate)
        {
            s_log.Info("Finding service instances with fabio adapter");
            return GetFabioResult();
        }

        public Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<RegistryInformation> predicate)
        {
            s_log.Info("Finding service instances with fabio adapter");
            return GetFabioResult();
        }
    }
}
