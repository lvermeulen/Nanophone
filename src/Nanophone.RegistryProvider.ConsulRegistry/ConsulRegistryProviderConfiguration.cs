using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.RegistryProvider.ConsulRegistry
{
    public class ConsulRegistryProviderConfiguration
    {
        public string ConsulHost { get; set; }
        public int ConsulPort { get; set; }
        public TimeSpan ReaperInterval { get; set; }
        public TimeSpan ReaperDelay { get; set; }
    }
}
