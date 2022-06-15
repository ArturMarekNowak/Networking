using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace PingPong
{
    public sealed class PingPong
    {
        public static async Task Main()
        {
            var pingSender = new Ping();
            var options = new PingOptions();

            var ipAddress = IPAddress.Parse("127.0.0.1");
            
            options.DontFragment = true;

            const string pingPayload = "abcdefghijklmnoprstqvxyz01234567";

            var pingPayloadBuffer = Encoding.ASCII.GetBytes(pingPayload);
            const int timeout = 120;
            
            Console.WriteLine ($"Pinging {ipAddress} with {pingPayload.Length} bytes of data:");

            const int packetsToSend = 4;
            var packetsReceived = 0;
            var packetsResponseTime = new List<long>();
            
            for (int i = 0; i < packetsToSend; i++)
            {
                PingReply reply;
                try
                {
                    reply = pingSender.Send(ipAddress, timeout, pingPayloadBuffer, options);
                }
                catch (Exception e)
                {
                    Console.WriteLine("PING: transmit failed. General error.");
                    await Task.Delay(1000);
                    continue;
                }
                
                switch (reply.Status)
                {
                    case IPStatus.Success:
                        
                        Console.WriteLine($"Reply from {ipAddress}: bytes={reply.Buffer.Length} time={reply.RoundtripTime} TTL={reply.Options.Ttl}");
                        packetsReceived++;
                        break;
                    
                    default:
                        
                        Console.WriteLine($"Reply from {ipAddress}: Destination host unreachable");
                        break;
                }

                packetsResponseTime.Add(reply.RoundtripTime);
                await Task.Delay(1000);
            }

            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine($"\nPing statistics for {ipAddress}:");
            stringBuilder.AppendLine($"\tPackets: Sent = {packetsToSend}, Received = {packetsReceived}, Lost = {packetsToSend - packetsReceived}({(1 - (double) packetsReceived / packetsToSend) * 100}% loss),");

            if (!packetsResponseTime.Any())
            {
                Console.WriteLine(stringBuilder.ToString());
                return;
            }

            stringBuilder.AppendLine($"Approximate round trip times in milli-seconds:");
            stringBuilder.AppendLine($"\tMinimum = {packetsResponseTime.Min()}ms, Maximum = {packetsResponseTime.Max()}ms, Average = {packetsResponseTime.Average()}ms");
            
            Console.WriteLine(stringBuilder.ToString());
        }
    }
}