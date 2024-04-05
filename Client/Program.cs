using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace Client
{
    public class Client
    {
        private bool isConnected;
        private TcpClient client;
        private StreamWriter sWriter;
        private StreamReader sReader;

        public Client(string ip, int port)
        {
            client = new TcpClient();
            client.Connect(ip, port);

            HandleCommunication();
        }

        public void HandleCommunication()
        {
            NetworkStream stream = client.GetStream();

            sWriter = new StreamWriter(stream, Encoding.ASCII);
            sReader = new StreamReader(stream, Encoding.ASCII);

            Console.WriteLine(sReader.ReadLine());
            string clientName = Console.ReadLine();

            sWriter.WriteLine(clientName);
            sWriter.Flush();

            isConnected = true;
            string sData = null;

            while (isConnected)
            {
                Console.Write($"{clientName} > ");
                sData = Console.ReadLine();

                if (sData.ToLower() == "exit")
                {
                    isConnected = false;
                    client.Close();
                }
                else
                {
                    sWriter.WriteLine(sData);
                    sWriter.Flush();

                    Console.WriteLine(sReader.ReadLine());
                }
            }

        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("127.0.0.1", 8080);
        }
    }
}
