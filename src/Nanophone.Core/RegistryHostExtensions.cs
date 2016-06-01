using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public static class RegistryHostExtensions
    {
        public static async Task<RegistryInformation> FindServiceInstanceAsync(this IRegistryHost registryHost, string serviceName)
        {
            var result = await registryHost.FindServiceInstancesAsync(serviceName).ConfigureAwait(false);
            if (result.Length == 0)
            {
                return default(RegistryInformation);
            }

            return result[ThreadLocalRandom.Current.Next(0, result.Length)];
        }

        public static async Task<RegistryInformation> FindServiceInstanceWithVersionAsync(this IRegistryHost registryHost, string serviceName, string version)
        {
            var serviceInstances = await registryHost.FindServiceInstancesAsync(serviceName).ConfigureAwait(false);
            var result = serviceInstances.Where(x => x.Version == version).ToArray();
            if (result.Length == 0)
            {
                return default(RegistryInformation);
            }

            return result[ThreadLocalRandom.Current.Next(0, result.Length)];
        }
    }
}