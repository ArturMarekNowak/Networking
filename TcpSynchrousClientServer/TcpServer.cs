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
            byte[] dataBUffer = new Byte[1024];
            
            //Starting the listener
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //Binding and setting up the connection queue to ten
                listener.Bind(localEndPoint);
                listener.Listen(10);

                //Beginning of listening
                while (true)
                {
                    Console.WriteLine("Listening...");

                    //Suspending program while waiting for incoming connection
                    Socket handler = listener.Accept();

                    receivedMessage = null;

                    while (true)
                    {
                        int bytesRec = handler.Receive(dataBUffer);
                        receivedMessage += Encoding.ASCII.GetString(dataBUffer, 0, bytesRec);
                        if (receivedMessage.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                        
                    }

                    Console.WriteLine("Message received : {0}", receivedMessage);

                    byte[] response = Encoding.ASCII.GetBytes(DateTime.Now.ToString(CultureInfo.InvariantCulture));

                    handler.Send(response);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception e)
            {
                listener.Close();
                Console.WriteLine(e.ToString());
                Console.WriteLine("Closed the server");
            }
            finally
            {
                listener.Close();
                Console.WriteLine("Closed the server");
            }
        }
    
    }
}