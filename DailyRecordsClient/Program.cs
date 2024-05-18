using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DailyRecordsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(host: "127.0.0.1", 8080);

            Console.WriteLine("Connected to server.");
            var receiveThread = new Thread(() => ReceiveData(clientSocket));
            receiveThread.Start();

            while (true)
            {
                Console.Write("Enter record content: ");
                var content = Console.ReadLine();
                var data = Encoding.ASCII.GetBytes("ADD " + content);
                clientSocket.Send(data);
            }
        }

        static void ReceiveData(Socket clientSocket)
        {
            var buffer = new byte[1024];
            while (true)
            {
                var received = clientSocket.Receive(buffer);
                var text = Encoding.ASCII.GetString(buffer, 0, received);
                Console.WriteLine($"Received from server: {text}");
            }
        }
    }
}
