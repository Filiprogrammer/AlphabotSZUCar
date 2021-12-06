using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AlphabotClientLibrary.Test.Tcp.Server
{
    class Program
    {
        /// <summary>
        /// Simple server application for testing the connection on tcp, 
        /// it uses the AddObstacleRequest and the NewObstacleRegisteredResponse for testing
        /// </summary>
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 9000);

            server.Start();
            Console.WriteLine("[SERVER]: Server started");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("[CONNECTION]: Client connected");
                NetworkStream ns = client.GetStream();

                while (client.Connected)
                {
                    byte[] msg = new byte[10];
                    ns.Read(msg, 0, msg.Length);
                    string receivedBytes = "";

                    foreach (var item in msg)
                    {
                        receivedBytes += (int)item + " -";
                    }

                    Console.WriteLine("[CONNECTION]: Received following bytes: " + receivedBytes);

                    Console.Write("[CONNECTION]: Put in the id you want to respond: ");

                    string sendId = Console.ReadLine();

                    ushort sendIdUShort = Convert.ToUInt16(sendId);
                    byte[] sendIdBytes = BitConverter.GetBytes(sendIdUShort);

                    byte[] send = { 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, msg[7], msg[6], sendIdBytes[0], sendIdBytes[1] };

                    ns.Write(send, 0, send.Length);
                    string sentBytes = "";

                    foreach (var item in send)
                    {
                        sentBytes += (int)item + " -";
                    }
                    Console.WriteLine("[CONNECTION]: Sent following bytes: " + sentBytes);
                }
            }
        }
    }
}
