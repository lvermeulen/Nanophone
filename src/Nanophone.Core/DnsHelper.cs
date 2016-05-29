using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
            //using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            //{
            //    socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            //    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
            //    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            //    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            //    int port = ((IPEndPoint)socket.LocalEndPoint).Port;
            //    socket.Close();
            //    return port;
            //}

            const int PORT_START = 5000;
            const int PORT_END = 10000;

            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            var activeListeners = ipProperties.GetActiveTcpListeners();
            var usedPorts = activeListeners.Select(p => p.Port).ToList();

            for (int port = PORT_START; port < PORT_END; port++)
            {
                if (!usedPorts.Contains(port))
                {
                    return port;
                }
            }

            return 0;
        }

        public static Uri GetNewLocalUri(int port = 0)
        {
            port = port == 0 ? GetFreeTcpPort() : port;
            var uri = new Uri($"http://localhost:{port}");

            return uri;
        }
    }
}
