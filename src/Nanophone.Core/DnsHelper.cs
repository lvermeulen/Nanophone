using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Nanophone.Core
{
    public static class DnsHelper
    {
        public static async Task<string> GetIpAddressAsync(bool ipv4 = true)
        {
            var hostEntry = await Dns.GetHostEntryAsync(string.Empty).ConfigureAwait(false);
            foreach (var address in hostEntry.AddressList)
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
