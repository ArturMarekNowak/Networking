using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Socket specification
            int port = 13;
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            EndPoint serverEndPoint = new IPEndPoint(ipAddress, port);

            //Data buffer
            String receivedMessage = null;
            byte[] dataBuffer = new Byte[1024];

            // Connect to a remote device.  
            try
            {
                //Starting the sender
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                try
                {
                    client.Connect(serverEndPoint);

                    byte[] response = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    client.SendTo(response, serverEndPoint);

                    int bytesRec = client.ReceiveFrom(dataBuffer, ref serverEndPoint);
                    Console.WriteLine("Server response: {0}", Encoding.ASCII.GetString(dataBuffer, 0, bytesRec));
                    client.Close();
                }
                catch (ArgumentNullException ane)
                {
                    client.Close();
                    Console.WriteLine("ArgumentNullException : {ane}", ane.ToString());
                }
                catch (SocketException se)
                {
                    client.Close();
                    Console.WriteLine("SocketException : {se}", se.ToString());
                }
                catch (Exception e)
                {
                    client.Close();
                    Console.WriteLine("Unexpected exception : {e}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}