using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP
{
    public sealed class TcpServer
    {
        public static async Task Main()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var ipEndpoint = new IPEndPoint(ipAddress, 12345);
            var tcpServer = new TcpListener(ipEndpoint);

            tcpServer.Start();
        
            var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
            
            Console.Write("Waiting for connections... ");
        
            var clientOne = await tcpServer.AcceptTcpClientAsync(timeout.Token);            
            Console.WriteLine("Client one connected!\nWaiting for client two...");
            
            var clientTwo = await tcpServer.AcceptTcpClientAsync(timeout.Token);            
            Console.WriteLine("Client two connected!");

            var tokenSource = new CancellationTokenSource();
        
            Task.WaitAny(new Task[]
            {
                Task.Run(() => TransferDataBetweenClients(clientOne, clientTwo, tokenSource)),
                Task.Run(() => TransferDataBetweenClients(clientTwo, clientOne, tokenSource)),
                Task.Run(() => ReadConsoleForUserCommands(tokenSource)),
            });
        }

        private static void ReadConsoleForUserCommands(CancellationTokenSource tokenSource)
        {
            var userInput = Console.ReadLine();
            while (userInput != "q" && userInput != "Q")
            {
                userInput = Console.ReadLine();
            }
                
            Console.WriteLine("Closing application...");
            tokenSource.Cancel();
        }

        private static void TransferDataBetweenClients(TcpClient sourceClient, TcpClient targetClient, CancellationTokenSource tokenSource)
        {
            var buffer = new byte[256];
            var sourceClientStream = sourceClient.GetStream();
            var targetClientStream = targetClient.GetStream();

            while (!tokenSource.Token.IsCancellationRequested)
            {
                int readBytes;
                while ((readBytes = sourceClientStream.Read(buffer)) != 0)
                {
                    var message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                    Console.WriteLine($"{DateTime.Now}: {message}");
                    targetClientStream.Write(Encoding.ASCII.GetBytes(message));
                }
                
                Console.WriteLine($"Client {sourceClient.Client.LocalEndPoint} disconnected");
                tokenSource.Cancel();
            }
        }
    }
}