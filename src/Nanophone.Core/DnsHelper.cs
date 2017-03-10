using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public static class DnsHelper
    {
        public static async Task<string> GetIpAddressAsync(IHostEntryProvider hostEntryProvider, bool ipv4 = true)
        {
            var hostEntry = await hostEntryProvider.GetHostEntryAsync().ConfigureAwait(false);
            foreach (var address in hostEntry.AddressList ?? Enumerable.Empty<IPAddress>())
            {
                if (ipv4 && address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }

                if (!ipv4 && address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    return address.ToString();
                }
            }

            return string.Empty;
        }
    }
}
