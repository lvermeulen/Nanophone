using System.Net;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Nanophone.Core.Tests
{
    public class DnsHelperShould
    {
        [Fact]
        public async Task GetIpv4AddressAsync()
        {
            var address = await DnsHelper.GetIpAddressAsync(new DnsHostEntryProvider());
            Assert.NotEmpty(address);
        }

        [Fact]
        public async Task GetIpv6AddressAsync()
        {
            var address = await DnsHelper.GetIpAddressAsync(new DnsHostEntryProvider(), ipv4: false);
            Assert.NotEmpty(address);
        }

        [Fact]
        public async Task BeEmptyWhenNoAddresses()
        {
            var hostEntryProvider = Substitute.For<IHostEntryProvider>();
            hostEntryProvider.GetHostEntryAsync().Returns(new IPHostEntry());
            var address = await DnsHelper.GetIpAddressAsync(hostEntryProvider);
            Assert.Empty(address);
        }
    }
}
