using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Nanophone.Core;

namespace Nanophone.RegistryHost.ConsulRegistry.Tests
{
    [TestFixture]
    public class StringExtensionsShould
    {
        const string PREFIX = "prefix-";

        [Test]
        public void TrimStartSingle()
        {
            string s = $"{PREFIX}string";
            Assert.AreEqual("string", s.TrimStart(PREFIX));
        }

        [Test]
        public void TrimStartMultiple()
        {
            string s = $"{PREFIX}{PREFIX}string";
            Assert.AreEqual("string", s.TrimStart(PREFIX));
        }

        [Test]
        public void TrimStartNone()
        {
            string s = "string";
            Assert.AreEqual("string", s.TrimStart(PREFIX));
        }

        [Test]
        public void TrimStartEmpty()
        {
            string s = "";
            Assert.AreEqual("", s.TrimStart(PREFIX));
        }

        [Test]
        public void TrimStartNull()
        {
            string s = null;
            Assert.AreEqual(null, s.TrimStart(PREFIX));
        }
    }
}
