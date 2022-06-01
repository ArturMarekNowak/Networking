using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
            
            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
        
            Task.WaitAny(new[]
            {
                Task.Run(() => ReadIncomingMessages(tcpClient, cancellationToken)),
                Task.Run(() => SendUserInputs(tcpClient, tokenSource, cancellationToken)),
                Task.Run(() => CheckConnectionPeriodically(tcpClient, tokenSource, cancellationToken)),
            });
            
            tcpClient.Close();
        }

        private static async Task CheckConnectionPeriodically(TcpClient tcpClient, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            while (tcpClient.Client.Connected && !tokenSource.Token.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }

            Console.WriteLine($"Server disconnected, caught in {MethodBase.GetCurrentMethod()}");
            if (!tcpClient.Client.Connected) tokenSource.Cancel();
        }
        
        private static async Task ReadIncomingMessages(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            var buffer = new byte[256];
            var stream = tcpClient.GetStream();

            while (!cancellationToken.IsCancellationRequested) 
            {
                int readBytes = stream.Read(buffer);
                if (readBytes == 0)
                {
                    await Task.Delay(100);
                    continue;
                }
                var message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                Console.WriteLine($"{DateTime.Now}: {message}");
            }

            Console.WriteLine($"Server disconnected, caught in {MethodBase.GetCurrentMethod()}");
        }

        private static void SendUserInputs(TcpClient tcpClient, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            var stream = tcpClient.GetStream();
            do
            {
                var userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput)) continue;
                if (userInput.Equals("q")) break;

                var bytes = Encoding.ASCII.GetBytes(userInput);
                stream.Write(bytes);

            } while (!tokenSource.Token.IsCancellationRequested);

            if (tokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Server disconnected, caught in {MethodBase.GetCurrentMethod()}");
            }
            else
            {
                Console.WriteLine("Closing application...");
                tokenSource.Cancel();
            }
        }
    }
}