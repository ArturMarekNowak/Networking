using System.Net.NetworkInformation;

namespace Netstat
{
    public sealed class Netstat
    {
        public static void Main()
        {
            var userInput = Console.ReadLine();
            switch (userInput)
            {
                case "-a":
                    ShowAllConnections();
                    break;
                case "-e":
                    ShowRoutingTable(); 
                    break;
                default:
                    Show();
                    break;
            }
        }

        private static void ShowRoutingTable()
        {
            Console.WriteLine("===========================================================================\nInterface List");
            
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()) 
                Console.WriteLine($"{ni.GetPhysicalAddress()} {ni.Description}");
        }

        private static void Show()
        {
            Console.WriteLine("Debug");
        }

        private static void ShowAllConnections()
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
