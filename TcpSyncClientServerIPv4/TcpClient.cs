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
            byte[] message, response;
            string userMessage;
            int bytesRec;

            // Connect to a remote device.  
            try
            {
                //Starting the sender
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    client.Connect(serverEndPoint);
                    
                    Console.WriteLine("Connected successfully to: " + client.RemoteEndPoint);
                    Console.WriteLine("Established client: " + client.LocalEndPoint);
                    Console.WriteLine("You can send messages now...");
                    
                    while ((userMessage = Console.ReadLine()) != "quit")
                    {
                        message = Encoding.ASCII.GetBytes(userMessage + "<EOF>");
                        client.Send(message);

                        bytesRec = client.Receive(dataBuffer);
                        Console.WriteLine("Server response: {0}", Encoding.ASCII.GetString(dataBuffer, 0, bytesRec));
                    }
                    
                    message = Encoding.ASCII.GetBytes("<EOF>");
                    client.Send(message);

                    bytesRec = client.Receive(dataBuffer);
                    Console.WriteLine("Server response: {0}", Encoding.ASCII.GetString(dataBuffer, 0, bytesRec));

                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (ArgumentNullException ane)
                {
                    client.Close();
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    client.Close();
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    client.Close();
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