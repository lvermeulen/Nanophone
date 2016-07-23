using Xunit;

namespace Nanophone.Core.Tests
{
    public class StringExtensionsShould
    {
        const string PREFIX = "prefix-";

        [Fact]
        public void TrimStartSingle()
        {
            string s = $"{PREFIX}string";
            Assert.Equal("string", s.TrimStart(PREFIX));
        }

        [Fact]
        public void TrimStartMultiple()
        {
            string s = $"{PREFIX}{PREFIX}string";
            Assert.Equal("string", s.TrimStart(PREFIX));
        }

        [Fact]
        public void TrimStartNone()
        {
            string s = "string";
            Assert.Equal("string", s.TrimStart(PREFIX));
        }

        [Fact]
        public void TrimStartEmpty()
        {
            string s = "";
            Assert.Equal("", s.TrimStart(PREFIX));
        }

        [Fact]
        public void TrimStartNull()
        {
            string s = null;
            Assert.Equal(null, s.TrimStart(PREFIX));
        }
    }
}
