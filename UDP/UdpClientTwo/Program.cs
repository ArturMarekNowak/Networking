using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDP
{
    public sealed class UdpClientTwo
    {
        public static void Main()
        {
            var clientEndpoint = new IPEndPoint(IPAddress.Loopback, 12347);
            var serverEndpoint = new IPEndPoint(IPAddress.Loopback, 12345);
            var udpClient = new UdpClient(clientEndpoint);
            
            var tokenSource = new CancellationTokenSource();
            
            Task.WaitAny(new []
            {
                Task.Run(() => ReadIncomingMessages(udpClient, serverEndpoint, tokenSource)),
                Task.Run(() => SendUserInputs(udpClient, serverEndpoint, tokenSource))
            });
            
            udpClient.Close();
        }

        private static void ReadIncomingMessages(UdpClient udpClient, IPEndPoint ipEndPoint, CancellationTokenSource tokenSource)
        {
            while (!tokenSource.Token.IsCancellationRequested)
            {
                var readBytes = udpClient.Receive(ref ipEndPoint);
                var message= Encoding.ASCII.GetString(readBytes, 0, readBytes.Length);
                Console.WriteLine($"{DateTime.Now}: {message}");
            }
            
            Console.WriteLine("Closing application...");
        }

        private static void SendUserInputs(UdpClient udpClient, IPEndPoint ipEndPoint, CancellationTokenSource tokenSource)
        {
            do
            {
                var userInput = Console.ReadLine();

                if (string.IsNullOrEmpty(userInput))
                    continue;

                if (userInput.Equals("q"))
                {
                    Console.WriteLine("Closing application...");
                    tokenSource.Cancel();
                }

                var bytes = Encoding.ASCII.GetBytes(userInput);
                udpClient.Send(bytes, bytes.Length, ipEndPoint);
            } while (!tokenSource.Token.IsCancellationRequested);
        }
    }
}