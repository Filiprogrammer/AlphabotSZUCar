using System;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Remote;

namespace Alphabot.Net.Cli
{
    class Program
    {
        static Prefs _prefs = Prefs.GetInstance();

        static void Main(string[] args)
        {
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
