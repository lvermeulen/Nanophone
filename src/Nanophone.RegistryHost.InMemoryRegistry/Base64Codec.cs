using System;
using System.Text;

namespace Nanophone.RegistryHost.InMemoryRegistry
{
    public class Base64Codec
    {
        public Encoding Encoding { get; set; }

        public Base64Codec()
        {
            Encoding = Encoding.UTF8;    
        }

        public string Encode(string s)
        {
            var bytes = Encoding.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }

        public byte[] EncodeToBytes(string s)
        {
            return Encoding.GetBytes(Encode(s));
        }

        public string Decode(string s)
        {
            var bytes = Convert.FromBase64String(s);
            return Encoding.GetString(bytes);
        }

        public string DecodeFromBytes(byte[] bytes)
        {
            return Decode(Encoding.GetString(bytes));
        }
    }
}
