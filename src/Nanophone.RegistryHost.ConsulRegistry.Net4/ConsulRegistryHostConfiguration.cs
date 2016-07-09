using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.RegistryHost.ConsulRegistry
{
    public class ConsulRegistryHostConfiguration
    {
        public string ConsulHost { get; set; }
        public int ConsulPort { get; set; }
        public TimeSpan CleanupInterval { get; set; }
        public TimeSpan CleanupDelay { get; set; }
        public bool IgnoreCriticalServices { get; set; }
        public Uri FabioUri { get; set; }

        public static ConsulRegistryHostConfiguration Default => new ConsulRegistryHostConfiguration
        {
            CleanupInterval = TimeSpan.MinValue,
            CleanupDelay = TimeSpan.MinValue
        };
    }
}
