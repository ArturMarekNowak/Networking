using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP
{
    public sealed class TcpClientOne
    {
        public static async Task Main()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var ipEndpoint = new IPEndPoint(ipAddress, 12346);
            var tcpClient = new TcpClient(ipEndpoint);
        
            await tcpClient.ConnectAsync(ipAddress, 12345);
            
            Task.WaitAny(new[]
            {
                Task.Run(() => ReadIncomingMessages(tcpClient)),
                Task.Run(() => SendUserInputs(tcpClient)),
            });
            
            tcpClient.Close();
        }

        private static void ReadIncomingMessages(TcpClient tcpClient)
        {
            var buffer = new byte[256];
            var stream = tcpClient.GetStream();

            try
            {
                int readBytes;
                while ((readBytes = stream.Read(buffer)) != 0)
                { 
                    var message = Encoding.ASCII.GetString(buffer, 0, readBytes); 
                    Console.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Server disconnected");
            }
        }

        private static void SendUserInputs(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();

            try
            {
                while (true)
                {
                    var userInput = Console.ReadLine();
                    if (string.IsNullOrEmpty(userInput)) continue;
                    if (userInput.Equals("q")) break;

                    var bytes = Encoding.ASCII.GetBytes(userInput);
                    stream.Write(bytes);
                }
                
                Console.WriteLine("Closing application...");
            }
            catch (IOException)
            {
                Console.WriteLine("Server disconnected");
            }
        }
    }
}