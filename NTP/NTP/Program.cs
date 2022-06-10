using System.Net;
using System.Net.Sockets;

namespace NTP
{
    public class NTP
    {
        public static void Main()
        {
            const string ntpServer = "time.windows.com";
            var address = Dns.GetHostEntry(ntpServer).AddressList.FirstOrDefault();

            if (address is null) throw new Exception("No ntp server found");

            var requestBytes = new byte[48];

            Console.WriteLine(address);
            
            // Based on RFC 2030:
            // Leap Indicator: First two bits - warning of an impending leap second to be inserted/deleted
            // Version Number: Middle three bits - indicating version number, 011 equal to IPv4 only
            // Mode values: Last three bits - indicating the mode, 011 equal to client mode
            requestBytes[0] = 0b00011011;
            
            var udpClient = new UdpClient();
            var ntpServerEndpoint = new IPEndPoint(address, 123);

            udpClient.Connect(ntpServerEndpoint);
            
            udpClient.Client.ReceiveTimeout = 3000;
            
            udpClient.Send(requestBytes);
            var responseBytes = udpClient.Receive(ref ntpServerEndpoint);
            udpClient.Close();
        }
    }
}