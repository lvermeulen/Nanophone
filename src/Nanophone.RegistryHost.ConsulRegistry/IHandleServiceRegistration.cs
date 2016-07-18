using System;
using System.Collections.Generic;
using Consul;

namespace Nanophone.RegistryHost.ConsulRegistry
{
    public interface IHandleServiceRegistration
    {
        AgentServiceRegistration Handle(AgentServiceRegistration registration, Uri uri, IEnumerable<KeyValuePair<string, string>> keyValuePairs);
    }
}
