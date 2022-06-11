using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NTP
{
    public sealed class NTP
    {
        public static void Main()
        {
            // Source: https://stackoverflow.com/a/12150289/15270760
            const string ntpServer = "time.windows.com";
            var address = Dns.GetHostEntry(ntpServer).AddressList.FirstOrDefault();

            if (address is null) throw new Exception("No ntp server found");

            var udpClient = new UdpClient();
            var ntpServerEndpoint = new IPEndPoint(address, 123);

            udpClient.Connect(ntpServerEndpoint);

            udpClient.Client.ReceiveTimeout = 3000;

            var tokenSource = new CancellationTokenSource();
            
            Task.WaitAny(new Task[]
            {
                Task.Run(() => DisplayNtpServerResponsePeriodically(udpClient, ntpServerEndpoint, tokenSource)),
                Task.Run(() => ReadUserInput(tokenSource)),
            });
            
            udpClient.Close();
        }

        private static void ReadUserInput(CancellationTokenSource tokenSource)
        {
            var userInput = Console.ReadLine();
            while (userInput != "q" && userInput != "Q")
            {
                userInput = Console.ReadLine();
            }
                
            Console.WriteLine("Closing application...");
            tokenSource.Cancel();
        }

        private static async Task DisplayNtpServerResponsePeriodically(UdpClient udpClient,
            IPEndPoint ntpServerEndpoint, CancellationTokenSource tokenSource)
        {
            while (!tokenSource.Token.IsCancellationRequested)
            {
                var bytesBuffer = new byte[48];
            
                // Based on RFC 2030:
                // Leap Indicator: First two bits - warning of an impending leap second to be inserted/deleted
                // Version Number: Middle three bits - indicating version number, 011 equal to IPv4 only
                // Mode values: Last three bits - indicating the mode, 011 equal to client mode
                bytesBuffer[0] = 0b00011011;
                
                udpClient.Send(bytesBuffer);
                bytesBuffer = udpClient.Receive(ref ntpServerEndpoint);

                var serverResponseReceivedDateTime = DateTime.Now;
                
                var ntpServerTimestamps = GetNtpTimeStamps(bytesBuffer);

                Console.WriteLine($"{ntpServerTimestamps.ToString(serverResponseReceivedDateTime)}");
                
                await Task.Delay(5000);
            }
        }

        private static NtpServerTimestamps GetNtpTimeStamps(byte[] bytesBuffer)
        {
            var fieldsIndexes = new int[] { 16, 24, 32, 40 };

            var timestampsArray = new DateTime[4];
            
            for (var i = 0; i < fieldsIndexes.Length; i++)
            {
                ulong seconds = BitConverter.ToUInt32(bytesBuffer, fieldsIndexes[i]);
                ulong secondsFraction = BitConverter.ToUInt32(bytesBuffer, fieldsIndexes[i] + 4);
                
                seconds = ConvertToLittleEndianUnsignedLong(seconds);
                secondsFraction = ConvertToLittleEndianUnsignedLong(secondsFraction);
                
                var milliseconds = (seconds * 1000) + ((secondsFraction * 1000) / 0x100000000L);
                
                var dateTime =
                    (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

                timestampsArray[i] = dateTime;
            }

            return new NtpServerTimestamps(timestampsArray);
        }

        // Source: stackoverflow.com/a/3294698/162671
        static uint ConvertToLittleEndianUnsignedLong(ulong x)
        {
            return (uint) (((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        private sealed class NtpServerTimestamps
        {
            private DateTime _referenceTimestamp { get; }
            private DateTime _originateTimestamp { get; }
            private DateTime _receiveTimestamp { get; }
            private DateTime _transmitTimestamp { get; }

            public NtpServerTimestamps(DateTime[] datetimeArray)
            {
                _referenceTimestamp = datetimeArray[0].ToLocalTime();
                _originateTimestamp = datetimeArray[1].ToLocalTime();
                _receiveTimestamp = datetimeArray[2].ToLocalTime();
                _transmitTimestamp = datetimeArray[3].ToLocalTime();
            }

            public string ToString(DateTime serverResponseReceivedDateTime)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------NTP Server Response-------------------");
                stringBuilder.AppendLine($"Reference: {_referenceTimestamp}:{_referenceTimestamp.Millisecond}");
                stringBuilder.AppendLine($"Originate: {_originateTimestamp}:{_originateTimestamp.Millisecond}");
                stringBuilder.AppendLine($"Receive: {_receiveTimestamp}:{_receiveTimestamp.Millisecond}");
                stringBuilder.AppendLine($"Transmit: {_transmitTimestamp}:{_transmitTimestamp.Millisecond}");
                stringBuilder.AppendLine($"Now: {serverResponseReceivedDateTime}:{serverResponseReceivedDateTime.Millisecond}");
                stringBuilder.AppendLine("");
                
                stringBuilder.AppendLine($"Reference-Now difference: {(serverResponseReceivedDateTime - _referenceTimestamp).TotalMilliseconds}");
                stringBuilder.AppendLine($"Originate-Now difference: {(serverResponseReceivedDateTime - _originateTimestamp).TotalMilliseconds}");
                stringBuilder.AppendLine($"Receive-Now difference: {(serverResponseReceivedDateTime - _receiveTimestamp).TotalMilliseconds}");
                stringBuilder.AppendLine($"Transmit-Now difference: {(serverResponseReceivedDateTime - _transmitTimestamp).TotalMilliseconds}");

                return stringBuilder.ToString();
            }
        }
    }
}