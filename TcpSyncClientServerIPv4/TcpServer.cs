using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpServer
{
    class Program
    { 
        public static void Main()
		{
			StartTcpListenerThread();
			Thread.Sleep(300);
			StartTcpSendingThread();
			Console.ReadLine();
		}

		static void StartTcpListenerThread()
		{
			var tcpListener = new TcpListener(IPAddress.Any, Port);
			tcpListener.Start();
			Thread tcpListenerThread = new Thread(() =>
			{
				while (true)
				{
					try
					{
						var bytes = new byte[1024];
						var currentConnection = tcpListener.AcceptTcpClient();
						var stream = currentConnection.GetStream();
						var numBytesReadFromStream = stream.Read(bytes, 0, bytes.Length);
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}

				}
			});
			tcpListenerThread.Start();
		}

		static void StartTcpSendingThread()
		{
			var tcpSendingThread = new Thread(() =>
			{
				var tcpClient = new TcpClient("localhost", Port);
				while (true)
				{
					try
					{
						tcpClient.Client.Send(new byte[] { 1, 2, 8 });
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
			});
			tcpSendingThread.Start();
		}
    
    }
}