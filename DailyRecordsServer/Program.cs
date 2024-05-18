using RecordLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DailyRecordsServer
{
    class Program
    {
        private static List<Socket> _clients = new List<Socket>();
        private static List<Record> _records = new List<Record>();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            StartServer();
        }

        static void StartServer()
        {
            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            serverSocket.Listen(10);

            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                var clientSocket = serverSocket.Accept();
                _clients.Add(clientSocket);
                Console.WriteLine("Client connected.");

                var clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();
            }
        }

        static void HandleClient(Socket clientSocket)
        {
            var buffer = new byte[1024];
            while (true)
            {
                try
                {
                    var received = clientSocket.Receive(buffer);
                    var text = Encoding.ASCII.GetString(buffer, 0, received);
                    if (text.StartsWith("ADD "))
                    {
                        var content = text.Substring(4);
                        var record = CreateRecord(content);
                        NotifyClients(record);
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("Client disconnected.");
                    _clients.Remove(clientSocket);
                    clientSocket.Close();
                    break;
                }
            }
        }

        static Record CreateRecord(string content)
        {
            var record = new Record
            {
                Id = _records.Count > 0 ? _records[_records.Count - 1].Id + 1 : 1,
                Content = content,
                Date = DateTime.Now.Date
            };
            _records.Add(record);

            var filePath = $"Records_{record.Date:yyyyMMdd}.txt";
            var recordContent = $"Id: {record.Id}, Content: {record.Content}, Date: {record.Date}\n";
            FileHelper.SaveRecordToFile(filePath, recordContent);

            Console.WriteLine($"Record created: {record.Id} - {record.Content}");

            return record;
        }

        static void NotifyClients(Record record)
        {
            var recordContent = $"Id: {record.Id}, Content: {record.Content}, Date: {record.Date}\n";
            var data = Encoding.ASCII.GetBytes(recordContent);

            foreach (var client in _clients)
            {
                client.Send(data);
            }
        }
    }
}
