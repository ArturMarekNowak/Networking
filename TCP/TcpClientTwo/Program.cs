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
        
            var taskArray = new Task[2];
            taskArray[0] = taskFactory.StartNew(() => ReadingThread(tcpClient, cancellationToken));
            taskArray[1] = taskFactory.StartNew(() => SendingThread(tcpClient, tokenSource, cancellationToken));

            Task.WaitAll(taskArray);

            stream.Close();
            tcpClient.Close();
        }
    
        private static async Task ReadingThread(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            var bytes = new byte[256];
            var stream = tcpClient.GetStream();

            while (!cancellationToken.IsCancellationRequested)
            {
                int i;
                while ((i = await stream.ReadAsync(bytes, cancellationToken)) != 0)
                {
                    var data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine($"{DateTime.Now}: {data}");
                }
            }
        }

        private static async Task SendingThread(TcpClient tcpClient, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            var stream = tcpClient.GetStream();
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var userInput = Console.ReadLine();

                if (userInput is null) 
                    continue;

                if (userInput.Equals("q"))
                    tokenSource.Cancel();

                var bytes = Encoding.ASCII.GetBytes(userInput);
                await stream.WriteAsync(bytes, cancellationToken);
            }
        }
    }
}