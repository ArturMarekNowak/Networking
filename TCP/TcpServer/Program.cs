using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class TcpServer
{
    public static void Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        var ipEndpoint = new IPEndPoint(ipAddress, 12345);
        var tcpServer = new TcpListener(ipEndpoint);
        
        tcpServer.Start();
        
        Console.Write("Waiting for a connection... ");
        
        TcpClient client = tcpServer.AcceptTcpClient();            
        Console.WriteLine("Connected!");
        
        TaskFactory taskFactory = new TaskFactory();
        var taskArray = new Task[2];
        taskArray[0] = taskFactory.StartNew(() => ReadingThread(client));
        taskArray[1] = taskFactory.StartNew(() => SendingThread(client));

        Task.WaitAll(taskArray);
        Console.WriteLine("Here");
    }

    private static void ReadingThread(TcpClient client)
    {
        Byte[] bytes = new Byte[256];
        String data = null;
        
        NetworkStream stream = client.GetStream();

        int i;

        // Loop to receive all the data sent by the client.
        while((i = stream.Read(bytes, 0, bytes.Length))!=0)
        {
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            Console.WriteLine("Received: {0}", data);
        }
    }

    private static void SendingThread(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        
        var data = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes("foo"), 0, 3);
        byte[] msg = Encoding.ASCII.GetBytes(data);
        stream.Write(msg, 0, msg.Length);
    }
}