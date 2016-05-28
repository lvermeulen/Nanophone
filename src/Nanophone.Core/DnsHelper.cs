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

        private static int GetFreeTcpPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }

        public static Uri GetNewLocalUri(int port = 0)
        {
            port = port == 0 ? GetFreeTcpPort() : port;
            var uri = new Uri($"http://localhost:{port}");

            return uri;
        }
    }
}
