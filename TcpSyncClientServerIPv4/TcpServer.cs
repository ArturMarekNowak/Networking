using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpServer
{
    class Program
    { 
        public static void Main(string[] args)
        {
            //Socket specification
            int port = 13;
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            
            //Data buffer
            String receivedMessage = null;
            byte[] dataBuffer = new Byte[1024];
            byte[] response;

            try
            {

                //Starting the listener
                Socket server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    //Binding and setting up the connection queue to ten
                    server.Bind(localEndPoint);
                    server.Listen(10);

                    Console.WriteLine("Established server: " + server.LocalEndPoint);
                    Console.WriteLine("Waiting for datagrams...");

                    //Beginning of listening
                    while (true)
                    {
                        Console.WriteLine("Listening...");

                        //Suspending program while waiting for incoming connection
                        Socket handler = server.Accept();
                        
                        receivedMessage = null;

                        while (true)
                        {
                            int bytesRec = handler.Receive(dataBuffer);
                            receivedMessage = Encoding.ASCII.GetString(dataBuffer, 0, bytesRec);

                            Console.WriteLine("Received message : {0}", receivedMessage);

                            if (receivedMessage.Equals("<EOF>"))
                            {
                                response = Encoding.ASCII.GetBytes("Closing connection");
                                handler.Send(response);
                                break;
                            }
                            
                            if (receivedMessage.IndexOf("<EOF>") > -1)
                            {
                                response = Encoding.ASCII.GetBytes(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                                handler.Send(response);
                            }
                        }

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();

                        break;
                    }
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
        }
    
    }
}