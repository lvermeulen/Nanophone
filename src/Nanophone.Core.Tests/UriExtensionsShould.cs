using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Nanophone.Core.Tests
{
    [TestFixture]
    public class UriExtensionsShould
    {
        [Test]
        public void GetHostAndPath()
        {
            var uri = new Uri("http://host.name:9234/path?param=0");
            Assert.AreEqual("host.name/path", uri.GetHostAndPath());
        }

        [Test]
        public void GetSchemeAndHost()
        {
            var uri = new Uri("http://host.name:9234/path?param=0");
            Assert.AreEqual("http://host.name", uri.GetSchemeAndHost());
        }
    }
}
