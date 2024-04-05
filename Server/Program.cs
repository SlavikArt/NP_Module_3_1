using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Server
    {
        private TcpListener server;
        private bool isRunning;

        public Server(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            isRunning = true;

            LoopClients();
        }

        public void LoopClients()
        {
            while (isRunning)
            {
                TcpClient newClient = server.AcceptTcpClient();

                Thread t = new Thread(HandleClient);
                t.Start(newClient);
            }
        }

        public void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;

            if (client == null)
            {
                Console.WriteLine("Client is null");
                return;
            }

            NetworkStream stream = client.GetStream();
            if (stream == null)
            {
                Console.WriteLine("Stream is null");
                return;
            }

            StreamWriter sWriter = new StreamWriter(stream, System.Text.Encoding.ASCII);
            StreamReader sReader = new StreamReader(stream, System.Text.Encoding.ASCII);

            sWriter.WriteLine("Server > Please enter your name:");
            sWriter.Flush();
            string clientName = sReader.ReadLine();

            bool bClientConnected = true;
            string sData = null;

            string clientEndPoint = client.Client.RemoteEndPoint.ToString();
            Console.WriteLine($"Client connected: {clientName} ({clientEndPoint}) at {DateTime.Now}");

            while (bClientConnected)
            {
                sData = sReader.ReadLine();

                if (sData != null)
                {
                    Console.WriteLine($"{clientName} > " + sData);

                    string[] currencies = sData.Split(' ');
                    if (currencies.Length == 2)
                    {
                        double exchangeRate = GetExchangeRate(currencies[0], currencies[1]);
                        if (exchangeRate != -1)
                        {
                            sWriter.WriteLine($"Server > {sData}: {exchangeRate}");
                            Console.WriteLine($"Exchange rate requested: {sData}: {exchangeRate}");
                        }
                        else
                        {
                            sWriter.WriteLine("Server > Unsupported currency pair");
                            Console.WriteLine($"Unsupported currency pair: '{sData}'");
                        }
                    }
                    else
                    {
                        sWriter.WriteLine("Server > Invalid request");
                        Console.WriteLine($"Invalid request: '{sData}'");
                    }
                    sWriter.Flush();
                }
                else
                {
                    bClientConnected = false;
                    Console.WriteLine($"Client disconnected: {clientName} ({clientEndPoint}) at {DateTime.Now}");
                }
            }
        }

        public double GetExchangeRate(string fromCurrency, string toCurrency)
        {
            Dictionary<string, double> exchangeRates = new()
            {
                { "USD_EUR", 0.85 },
                { "EUR_USD", 1.18 },
                { "USD_GBP", 0.76 },
                { "GBP_USD", 1.31 },
                { "USD_JPY", 110.25 },
                { "JPY_USD", 0.0091 },
                { "USD_CAD", 1.27 },
                { "CAD_USD", 0.79 },
                { "USD_AUD", 1.36 },
                { "AUD_USD", 0.73 }
            };

            string key = fromCurrency + "_" + toCurrency;
            if (exchangeRates.ContainsKey(key))
                return exchangeRates[key];
            else
                return -1;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(8080);
        }
    }
}
