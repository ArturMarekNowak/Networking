using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP
{
    public sealed class TcpClientTwo
    {
        public static async Task Main()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var ipEndpoint = new IPEndPoint(ipAddress, 12347);
            var tcpClient = new TcpClient(ipEndpoint);
        
            await tcpClient.ConnectAsync(ipAddress, 12345);
            
            var stream = tcpClient.GetStream();
        
            var taskFactory = new TaskFactory();
            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
        
            var taskArray = new Task[3];
            taskArray[0] = taskFactory.StartNew(() => ReadingThread(tcpClient, cancellationToken), cancellationToken);
            taskArray[1] = taskFactory.StartNew(() => SendingThread(tcpClient, tokenSource, cancellationToken), cancellationToken);
            taskArray[2] = taskFactory.StartNew(() => ConnectionStatusThread(tcpClient, tokenSource, cancellationToken), cancellationToken);

            Task.WaitAny(taskArray);

            stream.Close();
            tcpClient.Close();
        }

        private static void ConnectionStatusThread(TcpClient tcpClient, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!tcpClient.Client.Connected)
                {
                    Console.WriteLine("Server disconnected");
                    tokenSource.Cancel();
                }

                Task.Delay(1000);
            }
        }
        
        private static void ReadingThread(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            var bytes = new byte[256];
            var stream = tcpClient.GetStream();

            while (true)
            {
                int i;
                while ((i = stream.Read(bytes)) != 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Server disconnected");
                        return;
                    }
                    
                    var data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine($"{DateTime.Now}: {data}");
                }
            }
            
        }

        private static void SendingThread(TcpClient tcpClient, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            var stream = tcpClient.GetStream();
            string userInput = "";
            
            while (true)
            {
                userInput = Console.ReadLine();
                
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Server disconnected");
                    return;
                }
                
                if (string.IsNullOrEmpty(userInput))
                    continue;

                if (userInput.Equals("q"))
                {
                    Console.WriteLine("Closing application...");
                    tokenSource.Cancel();
                }

                var bytes = Encoding.ASCII.GetBytes(userInput);
                stream.Write(bytes);
            }
        }
    }
}