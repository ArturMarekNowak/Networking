using System;
using System.Linq;
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
            var taskFactory = new TaskFactory();
            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
    
            var taskArray = new Task[2];
            taskArray[0] = taskFactory.StartNew(() => MessagingTask(12344, 12347, tokenSource, cancellationToken), cancellationToken);
            taskArray[1] = taskFactory.StartNew(() => MessagingTask(12345, 12346, tokenSource, cancellationToken), cancellationToken);

            Task.WaitAny(taskArray);
        }

        private static void MessagingTask(int recevingPort, int sendingPort, CancellationTokenSource tokenSource, CancellationToken cancellationToken)
        {
            UdpClient receiver = new UdpClient(recevingPort);
            IPEndPoint remoteIp = null;
            UdpClient sender = new UdpClient();
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var bytes = receiver.Receive(ref remoteIp);
                var message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                Console.WriteLine($"{DateTime.Now}: {message}");
                
                sender.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Loopback, sendingPort));
            }
        }
    }
}