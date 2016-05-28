using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public static class RegistryProviderExtensions
    {
        public static async Task<RegistryInformation> FindServiceInstanceAsync(this IRegistryProvider registryProvider, string serviceName)
        {
            var result = await registryProvider.FindServiceInstancesAsync(serviceName).ConfigureAwait(false);
            if (result.Length == 0)
            {
                return default(RegistryInformation);
            }

            return result[ThreadLocalRandom.Current.Next(0, result.Length)];
        }

        public static async Task<RegistryInformation> FindServiceInstanceWithVersionAsync(this IRegistryProvider registryProvider, string serviceName, string version)
        {
            var serviceInstances = await registryProvider.FindServiceInstancesAsync(serviceName).ConfigureAwait(false);
            var result = serviceInstances.Where(x => x.Version == version).ToArray();
            if (result.Length == 0)
            {
                return default(RegistryInformation);
            }

            return result[ThreadLocalRandom.Current.Next(0, result.Length)];
        }
    }
}