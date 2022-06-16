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

            udpClient.Client.ReceiveTimeout = 5000;

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

        // Based on RFC 2030:
        // Leap Indicator: First two bits - warning of an impending leap second to be inserted/deleted
        // Version Number: Middle three bits - indicating version number, 011 equal to IPv4 only
        // Mode values: Last three bits - indicating the mode, 011 equal to client mode
        private static byte[] _requestBytes = new byte[] { 0b00011011 };
        
        private static async Task DisplayNtpServerResponsePeriodically(UdpClient udpClient,
            IPEndPoint ntpServerEndpoint, CancellationTokenSource tokenSource)
        {
            while (!tokenSource.Token.IsCancellationRequested)
            {
                udpClient.Send(_requestBytes);
                var bytesBuffer = udpClient.Receive(ref ntpServerEndpoint);

                var serverResponseReceivedDateTime = DateTime.Now;
                var ntpServerTimestamps = GetNtpTimeStamps(bytesBuffer);

                Console.WriteLine($"{ntpServerTimestamps.ToString(serverResponseReceivedDateTime)}");

                await Task.Delay(5000, tokenSource.Token);
            }
        }
        
        private static readonly DateTime baseTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly int[] fieldsIndexes = new int[] { 16, 24, 32, 40 };
        private const int MilliSecondMultiplier = 1_000;
        private const long MilliSecondDivider = 0x100_000_000;

        private static NtpServerTimestamps GetNtpTimeStamps(byte[] bytesBuffer)
        {
            var timestamps = new List<DateTime>(fieldsIndexes.Length);
            foreach (var index in fieldsIndexes)
            {
                ulong seconds = BitConverter.ToUInt32(bytesBuffer, index);
                ulong secondsFraction = BitConverter.ToUInt32(bytesBuffer, index + 4);

                seconds = ConvertToLittleEndianUnsignedLong(seconds);
                secondsFraction = ConvertToLittleEndianUnsignedLong(secondsFraction);

                var milliseconds = seconds * MilliSecondMultiplier + secondsFraction * MilliSecondMultiplier / MilliSecondDivider;
                timestamps.Add(baseTime.AddMilliseconds((long)milliseconds));
            }

            return new NtpServerTimestamps(timestamps[0], timestamps[1], timestamps[2], timestamps[3]);
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

            public NtpServerTimestamps(DateTime referenceTimestamp, DateTime originateTimestamp, DateTime receiveTimestamp, DateTime transmitTimestamp)
            {
                _referenceTimestamp = referenceTimestamp.ToLocalTime();
                _originateTimestamp = originateTimestamp.ToLocalTime();
                _receiveTimestamp = receiveTimestamp.ToLocalTime();
                _transmitTimestamp = transmitTimestamp.ToLocalTime();
            }
            
            private string FormatDateTimeToDisplay(string prefix, DateTime timestamp)
                => $"{prefix}: {timestamp}:{timestamp.Millisecond}";

            private string FormatDateTimesToDisplay(string prefix, DateTime received, DateTime timestamp)
                => $"{prefix}-Now difference: {(received - timestamp).TotalMilliseconds}";

            public string ToString(DateTime serverResponseReceivedDateTime) => string.Join(Environment.NewLine, new[]
            {
                    "-------------------NTP Server Response-------------------",
                    FormatDateTimeToDisplay("Reference", _referenceTimestamp),
                    FormatDateTimeToDisplay("Originate", _originateTimestamp),
                    FormatDateTimeToDisplay("Receive", _receiveTimestamp),
                    FormatDateTimeToDisplay("Transmit", _transmitTimestamp),
                    FormatDateTimeToDisplay("Now", serverResponseReceivedDateTime),
                    "",
                    FormatDateTimesToDisplay("Reference", serverResponseReceivedDateTime, _referenceTimestamp),
                    FormatDateTimesToDisplay("Originate", serverResponseReceivedDateTime, _originateTimestamp),
                    FormatDateTimesToDisplay("Receive", serverResponseReceivedDateTime, _receiveTimestamp),
                    FormatDateTimesToDisplay("Transmit", serverResponseReceivedDateTime, _transmitTimestamp)
            });
        }
    }
}