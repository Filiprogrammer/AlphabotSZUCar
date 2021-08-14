using System;
using System.Threading;
using Alphabot.Net.Car;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Car.Devices;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Car.Steering;
using Alphabot.Net.Remote;
using Alphabot.Net.Remote.Core;

namespace Alphabot.Net.Cli
{
    class Program
    {
        static Prefs _prefs = Prefs.GetInstance();

        static void Main(string[] args)

        {


            /*
            IAlphabotCar rc = RemoteCar.GetInstance().Current;

            Console.WriteLine("forward");
            rc.MoveBackward();
            Thread.Sleep(3000);
            for (int i = 20; i < 100; i++)
            {
                rc.SetSpeed(i);
                Thread.Sleep(100);
            }

            rc.Stop();
            Console.WriteLine("backward");
            rc.MoveBackward();
            
            for (int i = 20; i < 100; i++)
            {
                rc.SetSpeed(i);
                Thread.Sleep(100);
            }

            rc.Stop();

            Console.WriteLine("commandline starting");
            string command = "";
            while ((command = Console.ReadLine()) != "exit")
            {
                
                if (command == "forward")
                {
                    rc.MoveForward();
                }
                if (command == "stop")
                {
                    rc.Stop();
                }
                if (command == "left")
                {
                    rc.TurnLeft();
                }
                if (command == "speed")
                {
                    rc.SetSpeed(80);
                }
            }

            Console.WriteLine("press enter to exit");
            Console.ReadLine();
            */
        
            
            
            bool autostart = true;
            bool running = false;
            Console.WriteLine("Alphabot Remote Service");

            Console.WriteLine($"Running on: {Environment.OSVersion}");
            Console.WriteLine($"Machine:    {Environment.MachineName}");
            Console.WriteLine();
            Console.WriteLine("Available commands: start, stop, exit");
            Console.WriteLine();
            Console.WriteLine($"Autostart: {autostart}");

            TcpService service = new TcpService(_prefs.ServiceSettings.AlphabotServicePort);
            if (autostart)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(":> Alphabot Remote Service is starting ...");
                service.Start();
                running = true;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            string command = string.Empty;


            do
            {
                Console.Write(":> ");
                command = Console.ReadLine();

                if (command.ToLower().Trim() == "start")
                {
                    if (!running)
                    {
                        Console.WriteLine(":> Alphabot Remote Service is starting ...");
                        service.Start();
                    }
                    else
                    {
                        Console.WriteLine(":> Alphabot Remote Service is already running ...");
                    }
                }

                if (command.ToLower().Trim() == "stop")
                {
                    if (running)
                    {
                        Console.WriteLine(":> Alphabot Remote Service is stopping ...");
                        service.Stop();
                        running = false;
                    }
                    else
                    {
                        Console.WriteLine(":> Alphabot Remote Service is already stopped ...");
                    }
                }
            } while (command.ToLower().Trim() != "exit");
            
            

            Console.WriteLine(":> Alphabot Remote Service closed ...");
        }
    }
}
