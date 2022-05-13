using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class TcpClientOne
{
    public static async Task Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        var ipEndpoint = new IPEndPoint(ipAddress, 12346);
        var tcpClient = new TcpClient(ipEndpoint);
        
        tcpClient.Connect(ipAddress, 12345);
        
        Byte[] data = System.Text.Encoding.ASCII.GetBytes("message");
        
        NetworkStream stream = tcpClient.GetStream();
        
        TaskFactory taskFactory = new TaskFactory();
        var tokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = tokenSource.Token;
        
        var taskArray = new Task[2];
        taskArray[0] = taskFactory.StartNew(() => ReadingThread(tcpClient, cancellationToken));
        taskArray[1] = taskFactory.StartNew(() => SendingThread(tcpClient, cancellationToken));
        
        while (!(Console.ReadLine() == "q"))
        {
            tokenSource.Cancel();
        }
        
        Task.WaitAll(taskArray);
        
        /*stream.Write(data, 0, data.Length);

        Console.WriteLine("Sent: {0}", "message");
        
        data = new Byte[256];

        String responseData = String.Empty;

        Int32 bytes = stream.Read(data, 0, data.Length);
        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Console.WriteLine("Received: {0}", responseData);
        */

        // Close everything.
        stream.Close();
        tcpClient.Close();
    }
    
    private static void ReadingThread(TcpClient client, CancellationToken cancellationToken)
    {
        Byte[] bytes = new Byte[256];
        String data = null;
        
        NetworkStream stream = client.GetStream();

        int i;

        while (!cancellationToken.IsCancellationRequested)
        {
            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0}", data);
            }
        }
    }

    private static async Task SendingThread(TcpClient client, CancellationToken cancellationToken)
    {
        NetworkStream stream = client.GetStream();

        while (!cancellationToken.IsCancellationRequested)
        {
            var data = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes("bar"), 0, 3);
            byte[] msg = Encoding.ASCII.GetBytes(data);
            stream.Write(msg, 0, msg.Length);
            await Task.Delay(1500);
        }
    }
}