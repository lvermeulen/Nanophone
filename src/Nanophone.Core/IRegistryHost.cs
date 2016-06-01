using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IRegistryHost
    {
        Task<RegistryInformation[]> FindServiceInstancesAsync(string name);
        Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri);
        Task BootstrapClientAsync();
        Task KeyValuePutAsync(string key, object value);
        Task<T> KeyValueGetAsync<T>(string key);
    }
}
