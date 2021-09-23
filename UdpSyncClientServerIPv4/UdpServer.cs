using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Socket specification
            int port = 13;
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderRemote = (EndPoint)sender;
            
            //Data buffer
            String receivedMessage = null;
            byte[] dataBuffer = new Byte[1024];

            // Connect to a remote device.  
            try
            {
                //Starting the sender
                Socket server = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                try
                {
                    server.Bind(localEndPoint);
        
                    Console.WriteLine("Established server: " + server.LocalEndPoint);
                    Console.WriteLine("Waiting for datagrams...");

                    while (true)
                    {
                        while (true)
                        {
                            int bytesRec = server.ReceiveFrom(dataBuffer, ref senderRemote);
                            receivedMessage = Encoding.ASCII.GetString(dataBuffer, 0, bytesRec);
                            
                            Console.WriteLine("Received message: {0}", receivedMessage);
                            
                            if (receivedMessage.IndexOf("<EOF>") > -1)
                            {
                                break;
                            }
                        }

                        byte[] response = Encoding.ASCII.GetBytes(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                        server.SendTo(response, senderRemote);
                        
                        if (receivedMessage.Equals("<EOF>"))
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Closing server: " + server.LocalEndPoint);
                    server.Close();
                }
                catch (ArgumentNullException ane)
                {
                    server.Close();
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    server.Close();
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    server.Close();
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