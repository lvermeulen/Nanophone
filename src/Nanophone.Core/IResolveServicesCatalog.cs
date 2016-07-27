using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IResolveServicesCatalog
    {
        Task<IDictionary<string, string[]>> GetServicesCatalogAsync();
    }
}
