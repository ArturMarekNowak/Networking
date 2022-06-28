using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RFC3091
{
    public sealed class PiGen
    {
        // RFC 3091 states that ports utilized by the PiGenerator should be 314159 and 220007. However those numbers are beyond
        // the possible range of ports numbers so they were shortened by one digit.

        private readonly IPEndPoint _basedDigitGeneratorEndPoint = new(IPAddress.Loopback, 31415);
        private readonly IPEndPoint _approximateServiceEndPoint = new(IPAddress.Loopback, 22007);
        private readonly string _pi;
        private readonly string _piApproximation;
        
        public PiGen()
        {
            _pi = PiNumberExtension.GetPiValue();
            _piApproximation = PiNumberExtension.GetPiValueApproximation();
        }

        public void Start()
        {
            var tokenSource = new CancellationTokenSource();
            
            Task.WaitAny(new Task[]
            {
                Task.Run(() => RunTcpBasedPiGenerator(tokenSource)),
                Task.Run(() => RunUdpBasedPiGenerator(tokenSource)),
                Task.Run(() => RunPiGeneratorApproximationService(tokenSource)),
                Task.Run(() => ReadConsoleForUserCommands(tokenSource))
            });
        }

        private void ReadConsoleForUserCommands(CancellationTokenSource tokenSource)
        {
            var userInput = Console.ReadLine();
            while (userInput != "q" && userInput != "Q")
            {
                userInput = Console.ReadLine();
            }
                
            Console.WriteLine("Closing application...");
            tokenSource.Cancel();
        }

        private void RunUdpBasedPiGenerator(CancellationTokenSource cancellationTokenSource)
        {
            var sourceClient = new UdpClient(_basedDigitGeneratorEndPoint);
            var remoteIp = new IPEndPoint(IPAddress.Loopback, 12348);

            var pi = _pi.Remove(1, 1);
            
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var buffer = sourceClient.Receive(ref remoteIp);
                var piDigit = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                
                if (!int.TryParse(piDigit, out var index))
                {
                    Console.WriteLine("Payload is not an integer number");
                    continue;
                }
                
                var response = Encoding.ASCII.GetBytes($"{piDigit}:{pi[index]}");
                sourceClient.Send(response, response.Length, remoteIp);
                Console.WriteLine($"{DateTime.Now}: RunUdpBasedPiGenerator sent response: {response}");
            }
        }

        private async Task RunTcpBasedPiGenerator(CancellationTokenSource cancellationTokenSource)
        {
            var tcpServer = new TcpListener(_basedDigitGeneratorEndPoint);
            tcpServer.Start();

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var client = await tcpServer.AcceptTcpClientAsync();
                var tcpStream = client.GetStream();
                
                for (var i = 2; i < _pi.Length; i++)
                {
                    try
                    {
                        tcpStream.Write(Encoding.ASCII.GetBytes(_pi[i].ToString()));
                        Console.WriteLine($"{DateTime.Now}: RunTcpBasedPiGenerator sent response: {_pi[i]}");
                        await Task.Delay(1000);
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }

                tcpStream.Close();
            }
        }

        private async Task RunPiGeneratorApproximationService(CancellationTokenSource cancellationTokenSource)
        {
            var tcpServer = new TcpListener(_approximateServiceEndPoint);
            tcpServer.Start();
            
            var piApproximation = _piApproximation.ToString(CultureInfo.InvariantCulture);
            piApproximation = piApproximation.Remove(1, 1);
            
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var client = await tcpServer.AcceptTcpClientAsync();
                
                Console.WriteLine("Client connected");
                
                var tcpStream = client.GetStream();
                
                foreach (var digit in piApproximation)
                {
                    try
                    {
                        tcpStream.Write(Encoding.ASCII.GetBytes(digit.ToString()));
                        Console.WriteLine($"{DateTime.Now}: RunPiGeneratorApproximateService sent response: {digit}");
                        await Task.Delay(1000);
                    }
                    catch(Exception e)
                    {
                        break;
                    }
                }

                tcpStream.Close();
            }
        }
    }
}