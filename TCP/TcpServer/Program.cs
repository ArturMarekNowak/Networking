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
        
            Console.Write("Waiting for connections... ");
        
            var clientOne = await tcpServer.AcceptTcpClientAsync();            
            Console.WriteLine("Client one connected!\nWaiting for client two...");
            
            var clientTwo = await tcpServer.AcceptTcpClientAsync();            
            Console.WriteLine("Client two connected!");
        
            var taskFactory = new TaskFactory();
            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
        
            var taskArray = new Task[4];
            taskArray[0] = taskFactory.StartNew(() => MessagingTask(clientOne, clientTwo, cancellationToken));
            taskArray[1] = taskFactory.StartNew(() => MessagingTask(clientTwo, clientOne, cancellationToken));
            taskArray[2] = taskFactory.StartNew(() => ConnectionStatusThread(clientOne, tokenSource, cancellationToken));
            taskArray[3] = taskFactory.StartNew(() => ConnectionStatusThread(clientTwo, tokenSource, cancellationToken));

            while (Console.ReadLine() != "q")
            {
                tokenSource.Cancel();
            }
        
            Task.WaitAll(taskArray);
        }

        private static async void ConnectionStatusThread(TcpClient tcpClient, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!tcpClient.Client.Connected)
                {
                    Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} disconnected");
                    tokenSource.Cancel();
                    break;
                }

                await Task.Delay(10000);
            }
        }

        private static async Task MessagingTask(TcpClient clientOne, TcpClient clientTwo, CancellationToken cancellationToken)
        {
            var bytes = new byte[256];
            var clientOneStream = clientOne.GetStream();
            var clientTwoStream = clientTwo.GetStream();

            while (!cancellationToken.IsCancellationRequested)
            {
                int i;
                while ((i = await clientOneStream.ReadAsync(bytes, cancellationToken)) != 0)
                {
                    var data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine($"{DateTime.Now}: {data}");

                    var msg = Encoding.ASCII.GetBytes(data);
                    await clientTwoStream.WriteAsync(msg, cancellationToken);
                }
            }
        }
    }
}