using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.Core
{
    public class RegistryInformation
    {
        public string Address { get; }
        public int Port { get; }
        public string Version { get; }
        public IEnumerable<KeyValuePair<string, string>> KeyValuePairs { get; }

        public RegistryInformation(string serviceAddress, int servicePort)
        {
            Address = serviceAddress;
            Port = servicePort;
        }

        public RegistryInformation(string serviceAddress, int servicePort, string version, IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
            : this(serviceAddress, servicePort)
        {
            Version = version;
            KeyValuePairs = keyValuePairs;
        }
    }
}
