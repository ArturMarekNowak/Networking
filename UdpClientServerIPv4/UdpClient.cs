using System;
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
            byte[] dataBuffer = new Byte[1024];
            string userMessage;
            byte[] response;
            int bytesRec;

            // Connect to a remote device.  
            try
            {
                //Starting the client
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                try
                {
                    client.Connect(serverEndPoint);
                    
                    Console.WriteLine("Connected successfully to: " + client.RemoteEndPoint);
                    Console.WriteLine("Established client: " + client.LocalEndPoint);
                    Console.WriteLine("You can send messages now...");
                    
                    while ((userMessage = Console.ReadLine()) != "quit")
                    {
                        response = Encoding.ASCII.GetBytes(userMessage + "<EOF>");
                        client.SendTo(response, serverEndPoint);

                        bytesRec = client.ReceiveFrom(dataBuffer, ref serverEndPoint);
                        Console.WriteLine("Server response: {0}", Encoding.ASCII.GetString(dataBuffer, 0, bytesRec));
                    }
                    
                    response = Encoding.ASCII.GetBytes("<EOF>");
                    client.SendTo(response, serverEndPoint);

                    bytesRec = client.ReceiveFrom(dataBuffer, ref serverEndPoint);
                    Console.WriteLine("Server response: {0}", Encoding.ASCII.GetString(dataBuffer, 0, bytesRec));

                    Console.WriteLine("Closing client: " + client.LocalEndPoint);
                    client.Close();
                }
                catch (ArgumentNullException ane)
                {
                    client.Close();
                    Console.WriteLine("ArgumentNullException : {0}", ane);
                }
                catch (SocketException se)
                {
                    client.Close();
                    Console.WriteLine("SocketException : {0}", se);
                }
                catch (Exception e)
                {
                    client.Close();
                    Console.WriteLine("Unexpected exception : {0}", e);
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