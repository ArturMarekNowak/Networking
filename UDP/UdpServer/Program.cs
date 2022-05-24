using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDP
{
    public sealed class UdpServer
    {
        public static void Main()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var ipEndpoint = new IPEndPoint(ipAddress, 12345);
            var udpServer = new UdpClient(ipEndpoint);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            
            var taskFactory = new TaskFactory();
            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
        
            var taskArray = new Task[1];
            taskArray[0] = taskFactory.StartNew(() => MessagingTask(udpServer, sender, tokenSource, cancellationToken), cancellationToken);

            Task.WaitAny(taskArray);
        }

        private static void MessagingTask(UdpClient udpServer, IPEndPoint sender, CancellationTokenSource tokenSource,
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var bytes = udpServer.Receive(ref sender);

                Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                udpServer.Send(bytes, bytes.Length, sender);
            }
        }
    }
}