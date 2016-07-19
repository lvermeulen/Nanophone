using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IRegistryHost : IResolveServiceInstances
    {
        Task RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<KeyValuePair<string, string>> keyValuePairs = null);
        Task StartClientAsync();
        Task KeyValuePutAsync(string key, object value);
        Task<T> KeyValueGetAsync<T>(string key);
        Task KeyValueDeleteAsync(string key);
        Task KeyValueDeleteTreeAsync(string prefix);
    }
}
