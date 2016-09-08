using System.Threading.Tasks;
using Xunit;

namespace Nanophone.Core.Tests
{
    public class DnsHelperShould
    {
        [Fact]
        public async Task GetIpv4AddressAsync()
        {
            var address = await DnsHelper.GetIpAddressAsync();
            Assert.NotEmpty(address);
        }

        [Fact]
        public async Task GetIpv6AddressAsync()
        {
            var address = await DnsHelper.GetIpAddressAsync(ipv4: false);
            Assert.NotEmpty(address);
        }
    }
}
