using System;
using Xunit;

namespace Nanophone.Core.Tests
{
    public class UriExtensionsShould
    {
        [Fact]
        public void GetPath()
        {
            var uri = new Uri("http://host.name:9234/path?param=0");
            Assert.Equal("path", uri.GetPath());
        }

        [Fact]
        public void GetHostAndPath()
        {
            var uri = new Uri("http://host.name:9234/path?param=0");
            Assert.Equal("host.name/path", uri.GetHostAndPath());
        }

        [Fact]
        public void GetSchemeAndHost()
        {
            var uri = new Uri("http://host.name:9234/path?param=0");
            Assert.Equal("http://host.name", uri.GetSchemeAndHost());
        }
    }
}
