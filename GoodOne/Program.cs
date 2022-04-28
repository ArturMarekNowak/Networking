using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class MyTcpListener
{
    public static void Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        var tcpServer = new TcpListener(ipAddress, 12345);
        
        tcpServer.Start();
        
        Console.Write("Waiting for a connection... ");
        
        TcpClient client = tcpServer.AcceptTcpClient();            
        Console.WriteLine("Connected!");
        
        TaskFactory taskFactory = new TaskFactory();
        var taskArray = new Task[2];
        taskArray[0] = taskFactory.StartNew(() => ReadingThread());
        taskArray[1] = taskFactory.StartNew(() => SendingThread(client));

        Task.WaitAll(taskArray);
        Console.WriteLine("Here");
    }

    private static void ReadingThread()
    {
        for (int i = 0; i < 100; i++)
        {
            Console.WriteLine("1");
            Task.Delay(1);
        }
    }

    private static void SendingThread(TcpClient client)
    {
        for (int i = 0; i < 100; i++)
        {
            Console.WriteLine("2");
            Task.Delay(1);
        }
    }
}