using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class TcpClientOne
{
    public static void Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        var ipEndpoint = new IPEndPoint(ipAddress, 12346);
        var tcpClient = new TcpClient(ipEndpoint);
        
        tcpClient.Connect(ipAddress, 12345);
        
        Byte[] data = System.Text.Encoding.ASCII.GetBytes("message");
        
        NetworkStream stream = tcpClient.GetStream();
        
        stream.Write(data, 0, data.Length);

        Console.WriteLine("Sent: {0}", "message");
        
        data = new Byte[256];

        String responseData = String.Empty;

        Int32 bytes = stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Console.WriteLine("Received: {0}", responseData);

        // Close everything.
        stream.Close();
        tcpClient.Close();
    }
}