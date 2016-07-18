using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IResolveServiceInstances
    {
        Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name);
        Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version);
    }
}
