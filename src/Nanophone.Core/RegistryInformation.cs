using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.Core
{
    public class RegistryInformation
    {
        public string Name { get; }
        public string Address { get; }
        public int Port { get; }
        public string Version { get; }
        public IEnumerable<KeyValuePair<string, string>> KeyValuePairs { get; }

        public RegistryInformation(string name, string serviceAddress, int servicePort)
        {
            Name = name;
            Address = serviceAddress;
            Port = servicePort;
        }

        public RegistryInformation(string name, string serviceAddress, int servicePort, string version, IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
            : this(name, serviceAddress, servicePort)
        {
            Version = version;
            KeyValuePairs = keyValuePairs;
        }
    }
}
