using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PiGen
{
    public class PiGen
    {
        // RFC 3091 states that ports utilized by the PiGen should be 314159 and 220007. However those numbers are beyond
        // the possible range of ports numbers so they were shortened by one digit.

        private readonly IPEndPoint _basedDigitGeneratorEndPoint = new(IPAddress.Loopback, 31415);
        private readonly IPEndPoint _approximateServiceEndPoint = new(IPAddress.Loopback, 22007);
        
        public PiGen()
        {

        }

        public void Start()
        {
            Task.WaitAll(new Task[]
            {
                Task.Run(() => TCPBasedGenerator()),
                Task.Run(() => UDPBasedGenerator()),
                Task.Run(() => ApproximateService()),
            });
        }

        private void UDPBasedGenerator()
        {
            UdpClient sourceClient = new UdpClient(_basedDigitGeneratorEndPoint);
            IPEndPoint? remoteIp = null;
            
            var buffer = sourceClient.Receive(ref remoteIp);
            var piDigit = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

            Int32.TryParse(piDigit, out var index);
            
            var pi = Math.PI.ToString().Remove(0, 2);

            var response = Encoding.ASCII.GetBytes($"{piDigit}:{pi[index]}");
            
            sourceClient.Send(response, response.Length, _basedDigitGeneratorEndPoint);
        }

        private async Task TCPBasedGenerator()
        {
            var tcpServer = new TcpListener(_basedDigitGeneratorEndPoint);
            
            tcpServer.Start();
            
            var client = await tcpServer.AcceptTcpClientAsync();
            var tcpStream = client.GetStream();

            var pi = Math.PI.ToString();
            
            for (var i = 2; i < pi.Length; i++)
            {
                tcpStream.Write(Encoding.ASCII.GetBytes(pi[i].ToString()));
                await Task.Delay(1000);
            }
        }
        
        private async Task ApproximateService()
        {
            var tcpServer = new TcpListener(_approximateServiceEndPoint);
            
            tcpServer.Start();
            
            var client = await tcpServer.AcceptTcpClientAsync();
            var tcpStream = client.GetStream();

            var pi = (22.0 / 7.0).ToString();

            pi = pi.Remove(1);
            
            for (var i = 0; i < pi.Length; i++)
            {
                tcpStream.Write(Encoding.ASCII.GetBytes(pi[i].ToString()));
                await Task.Delay(1000);
            }
        }
    }
}