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

            Task.WaitAll(taskArray);

            stream.Close();
            tcpClient.Close();
        }

        private static async Task ConnectionStatusThread(TcpClient tcpClient, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!tcpClient.Client.Connected)
                {
                    Console.WriteLine("Server disconnected");
                    tokenSource.Cancel();
                }

                await Task.Delay(10000);
            }
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
            string userInput;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                
                userInput = Reader.ReadLine(1000);

                if (userInput.Equals("q"))
                    tokenSource.Cancel();

                var bytes = Encoding.ASCII.GetBytes(userInput);
                await stream.WriteAsync(bytes, cancellationToken);
            }
        }
        
        public static class Reader 
        {
            private static readonly AutoResetEvent _getInput, _gotInput;
            private static string _input;

            static Reader() {
                _getInput = new AutoResetEvent(false);
                _gotInput = new AutoResetEvent(false);
                var inputThread = new Thread(ReaderThread);
                inputThread.IsBackground = true;
                inputThread.Start();
            }

            private static void ReaderThread() 
            {
                while (true) {
                    _getInput.WaitOne();
                    _input = Console.ReadLine();
                    _gotInput.Set();
                }
            }

            public static string ReadLine(int timeOutMillisecs = Timeout.Infinite) 
            {
                _getInput.Set();
                bool success = _gotInput.WaitOne(timeOutMillisecs);
                
                if (success)
                    return _input;
                
                throw new TimeoutException("User did not provide input within the timelimit.");
            }
        }
    }
}