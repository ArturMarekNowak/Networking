using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Netstat
{
    public sealed class Netstat
    {
        public static void Main()
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            Console.WriteLine("Proto Local Address Foreign Address State");

            foreach (var connection in ipGlobalProperties.GetActiveTcpConnections())
                Console.WriteLine($"TCP {connection.LocalEndPoint} {connection.RemoteEndPoint} {connection.State}");

            foreach (var tcpListener in ipGlobalProperties.GetActiveTcpListeners())
                Console.WriteLine($"TCP {tcpListener} Listening");

            foreach (var udpListener in ipGlobalProperties.GetActiveUdpListeners())
                Console.WriteLine($"UDP {udpListener} Listening");
        }
    }
}
