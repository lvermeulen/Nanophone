using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.RegistryProvider.ConsulRegistry
{
    public class ConsulRegistryProviderConfiguration
    {
        public string ConsulHost { get; set; }
        public int ConsulPort { get; set; }
        public TimeSpan CleanupInterval { get; set; }
        public TimeSpan CleanupDelay { get; set; }

        public static ConsulRegistryProviderConfiguration Default => new ConsulRegistryProviderConfiguration
        {
            CleanupInterval = TimeSpan.MinValue,
            CleanupDelay = TimeSpan.MinValue
        };
    }
}
