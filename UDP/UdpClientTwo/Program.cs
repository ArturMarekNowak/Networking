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
            
            var taskFactory = new TaskFactory();
            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
        
            var taskArray = new Task[2];
            taskArray[0] = taskFactory.StartNew(() => ReadingThread(udpClient, serverEndpoint, cancellationToken), cancellationToken);
            taskArray[1] = taskFactory.StartNew(() => SendingThread(udpClient, serverEndpoint, tokenSource, cancellationToken), cancellationToken);
            
            Task.WaitAny(taskArray);
            
            udpClient.Close();
        }

        private static void ReadingThread(UdpClient udpClient, IPEndPoint ipEndPoint, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Closing application...");
                    return;
                }
               
                var data = udpClient.Receive(ref ipEndPoint);
                var message= Encoding.ASCII.GetString(data, 0, data.Length);
                Console.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        private static void SendingThread(UdpClient udpClient, IPEndPoint ipEndPoint, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            string userInput = "";
            
            while (true)
            {
                userInput = Console.ReadLine();
                
                if (string.IsNullOrEmpty(userInput))
                    continue;

                if (userInput.Equals("q"))
                {
                    Console.WriteLine("Closing application...");
                    tokenSource.Cancel();
                }

                var bytes = Encoding.ASCII.GetBytes(userInput);
                udpClient.Send(bytes, bytes.Length, ipEndPoint);
            }
        }
    }
}