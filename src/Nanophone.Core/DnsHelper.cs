using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Nanophone.Core
{
    public static class DnsHelper
    {
        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntryAsync(Dns.GetHostName()).Result;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return null;
        }

        public static string GetLocalIpAddressEscaped(string escape = "_")
        {
            return GetLocalIpAddress().Replace(".", escape);
        }
    }
}
