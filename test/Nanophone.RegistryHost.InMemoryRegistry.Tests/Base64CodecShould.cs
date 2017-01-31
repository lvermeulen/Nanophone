using System.Text;
using Xunit;

namespace Nanophone.RegistryHost.InMemoryRegistry.Tests
{
    public class Base64CodecShould
    {
        private const string SOURCE = nameof(Base64CodecShould);
        private const string TARGET = "QmFzZTY0Q29kZWNTaG91bGQ=";

        private readonly Base64Codec _codec = new Base64Codec();
        private readonly byte[] _targetBytes = Encoding.UTF8.GetBytes(TARGET);

        [Fact]
        public void EncodeToString()
        {
            string result = _codec.Encode(SOURCE);
            Assert.Equal(TARGET, result);
        }

        [Fact]
        public void DecodeFromString()
        {
            string result = _codec.Decode(TARGET);
            Assert.Equal(SOURCE, result);
        }

        [Fact]
        public void EncodeToBytes()
        {
            var result = _codec.EncodeToBytes(SOURCE);
            Assert.Equal(_targetBytes, result);
        }

        [Fact]
        public void DecodeFromBytes()
        {
            string result = _codec.DecodeFromBytes(_targetBytes);
            Assert.Equal(SOURCE, result);
        }
    }
}
