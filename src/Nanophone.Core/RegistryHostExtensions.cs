using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public static class RegistryHostExtensions
    {
        private static RegistryInformation Random(IList<RegistryInformation> instances)
        {
            if (instances == null || !instances.Any())
            {
                return default(RegistryInformation);
            }

            return instances[ThreadLocalRandom.Current.Next(0, instances.Count)];
        }

        public static async Task<RegistryInformation> FindServiceInstanceAsync(this IRegistryHost registryHost, string serviceName)
        {
            var instances = await registryHost.FindServiceInstancesAsync(serviceName).ConfigureAwait(false);
            return Random(instances);
        }

        public static async Task<RegistryInformation> FindServiceInstanceWithVersionAsync(this IRegistryHost registryHost, string serviceName, string version)
        {
            var instances = await registryHost.FindServiceInstancesWithVersionAsync(serviceName, version).ConfigureAwait(false);
            return Random(instances);
        }
    }
}