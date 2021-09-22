using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Socket specification
            int port = 13;
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, port);

            //Data buffer
            byte[] dataBuffer = new Byte[1024];

            // Connect to a remote device.  
            try
            {
                //Starting the sender
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(serverEndPoint);

                    Console.WriteLine("Connected successfully to {0}", sender.RemoteEndPoint);

                    byte[] message = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    int bytesSent = sender.Send(message);

                    int bytesRec = sender.Receive(dataBuffer);
                    Console.WriteLine("Server response: {0}", Encoding.ASCII.GetString(dataBuffer, 0, bytesRec));

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    sender.Close();
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    sender.Close();
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    sender.Close();
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
			
			Console.WriteLine("\nPress ENTER to continue...");  
			Console.Read();
        }
    }
}