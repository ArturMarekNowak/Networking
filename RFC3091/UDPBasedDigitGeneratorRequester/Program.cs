using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RFC3091
{
    public sealed class Program
    {
        public static void Main()
        {
            var clientEndpoint = new IPEndPoint(IPAddress.Loopback, 12348);
            var basedDigitGeneratorEndPoint = new IPEndPoint(IPAddress.Loopback, 31415);
            var udpClient = new UdpClient(clientEndpoint);
            
            var tokenSource = new CancellationTokenSource();
            
            Task.WaitAny(new []
            {
                Task.Run(() => ReadIncomingMessages(udpClient, basedDigitGeneratorEndPoint, tokenSource)),
                Task.Run(() => SendUserInputs(udpClient, basedDigitGeneratorEndPoint, tokenSource))
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
                {
                    Console.WriteLine("Input must be an integer number!!!");
                    continue;
                }

                if (userInput.Equals("q"))
                {
                    Console.WriteLine("Closing application...");
                    tokenSource.Cancel();
                    break;
                }
                
                if (!int.TryParse(userInput, out var number))
                {
                    Console.WriteLine("Input must be an integer number!!!");
                    continue;
                }

                var bytes = Encoding.ASCII.GetBytes(userInput);
                udpClient.Send(bytes, bytes.Length, ipEndPoint);
                
            } while (!tokenSource.Token.IsCancellationRequested);
        }
    }
}