using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanophone.Core
{
    public class RegistryInformation
    {
        public string Address { get; }
        public int Port { get; }
        public IEnumerable<KeyValuePair<string, string>> KeyValuePairs { get; }

        public RegistryInformation(string serviceAddress, int servicePort, IEnumerable<KeyValuePair<string, string>> keyValuePairs = null)
        {
            Address = serviceAddress;
            Port = servicePort;
            KeyValuePairs = keyValuePairs;
        }
    }
}
