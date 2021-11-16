using AlphabotClientLibrary.Core;
using AlphabotClientLibrary.Core.Tcp;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;
using AlphabotClientLibrary.Shared.Requests;
using AlphabotClientLibrary.Shared.Responses;
using System;
using System.Net;

namespace AlphabotClientLibrary.Test.Tcp.Client
{
    class Program
    {
        /// <summary>
        /// Simple client application for testing the connection on tcp, 
        /// it uses the AddObstacleRequest and the NewObstacleRegisteredResponse for testing
        /// </summary>
        static void Main(string[] args)
        {
            ConnectionHandler ch = new TcpHandlerWindows();

            ch.ResponseHandler.AddResponseListener(Receive);

            ch.Connect(new WiFiConnectionData(IPAddress.Loopback, 9000));
            Console.WriteLine("[CLIENT]: Connected to server");

            Console.Write("[CONNECTION]: To send a AddObstacleRequest, put in the height of the obstacle you want: ");
            string input = Console.ReadLine();
            ushort height = Convert.ToUInt16(input);

            Obstacle obs = new Obstacle(new Position(0, 0), 1, height);

            ch.SendAction(new AddObstacleRequest(obs));
            Console.WriteLine("[CONNECTION]: Request was sent");

            Console.ReadKey();

            ch.Disconnect();
            Console.ReadKey();
        }

        public static void Receive(IAlphabotResponse response)
        {
            if(response is NewObstacleRegisteredResponse)
            {
                NewObstacleRegisteredResponse res = response as NewObstacleRegisteredResponse;

                ushort height = res.Obstacle.Height;
                ushort id = res.Obstacle.Id;

                Console.WriteLine("[CONNECTION]: NewObstacleResponse on height " + height + " with id " + id);
            }
            else
            {
                Console.WriteLine("[CONNECTION]: An unknown packet id was received");
            }
        }
    }
}
