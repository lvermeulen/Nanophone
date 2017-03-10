using System.Net;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public interface IHostEntryProvider
    {
        Task<IPHostEntry> GetHostEntryAsync();
    }
}
