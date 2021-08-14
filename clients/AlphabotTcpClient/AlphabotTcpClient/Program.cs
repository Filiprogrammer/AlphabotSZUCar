using System;
using System.Net.Sockets;

namespace AlphabotTcpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string address = "127.0.0.1";
            ushort port = 9000;

            if (args.Length >= 1)
                address = args[0];

            if (args.Length >= 2)
                try
                {
                    port = ushort.Parse(args[1]);
                }
                catch (Exception) { }

            Console.WriteLine("Connecting...");
            AlphabotClient client = new AlphabotClient(address, port);
            bool connected = false;

            try
            {
                client.Connect();
                connected = true;
            }
            catch (SocketException)
            {
                Console.WriteLine("Connection refused");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

            if (!connected)
                return;

            Console.WriteLine("Connected");

            for (; ; )
            {
                Console.Write("Enter text to send: ");
                string msg = Console.ReadLine();

                if (msg.Length == 0)
                    continue;

                client.SendString(msg);
            }
        }
    }
}
