using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RFC3091
{
    public sealed class Program
    {
        public static async Task Main()
        {
            var basedDigitGeneratorEndPoint = new IPEndPoint(IPAddress.Loopback, 31415);
            
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, 12347);
            var tcpClient = new TcpClient(ipEndpoint);
        
            await tcpClient.ConnectAsync(basedDigitGeneratorEndPoint);

            Task.WaitAny(new Task[]
            {
                Task.Run(() => ReadConsoleForUserCommands()),
                Task.Run(() => ReadIncomingMessages(tcpClient)),
            });
            
            tcpClient.Close();
        }

        private static void ReadConsoleForUserCommands()
        {
            var userInput = Console.ReadLine();
            while (userInput != "q" && userInput != "Q")
            {
                userInput = Console.ReadLine();
            }
                
            Console.WriteLine("Closing application...");
        }

        private static void ReadIncomingMessages(TcpClient tcpClient)
        {
            var buffer = new byte[256];
            var stream = tcpClient.GetStream();

            try
            {
                int readBytes;
                while ((readBytes = stream.Read(buffer)) != 0)
                { 
                    var message = Encoding.ASCII.GetString(buffer, 0, readBytes); 
                    Console.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Server disconnected");
            }
        }
    }
}