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
            var tokenSource = new CancellationTokenSource();

            Task.WaitAny(new Task[]
            {
                Task.Run(() => TransferDataBetweenClients(12344, 12347, tokenSource)),
                Task.Run(() => TransferDataBetweenClients(12345, 12346, tokenSource)),
            });
        }

        private static void TransferDataBetweenClients(int sourcePort, int targetPort, CancellationTokenSource tokenSource)
        {
            UdpClient sourceClient = new UdpClient(sourcePort);
            IPEndPoint remoteIp = null;
            UdpClient targetClient = new UdpClient();
            
            while (!tokenSource.Token.IsCancellationRequested)
            {
                var buffer = sourceClient.Receive(ref remoteIp);
                var message = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

                Console.WriteLine($"{DateTime.Now}: {message}");
                
                targetClient.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Loopback, targetPort));
            }
        }
    }
}