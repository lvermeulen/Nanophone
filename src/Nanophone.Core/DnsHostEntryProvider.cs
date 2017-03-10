using System.Net;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public class DnsHostEntryProvider : IHostEntryProvider
    {
        public async Task<IPHostEntry> GetHostEntryAsync() => await Dns.GetHostEntryAsync(string.Empty).ConfigureAwait(false);
    }
}
