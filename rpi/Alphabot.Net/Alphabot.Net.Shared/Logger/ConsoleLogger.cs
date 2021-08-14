using System;
using Alphabot.Net.Shared.Contracts;

namespace Alphabot.Net.Shared.Logger
{
    public class ConsoleLogger: IServiceLogger
    {
        public ConsoleLogger()
        {
            LogFromLevel = LogLevel.Debug;
        }
        public LogLevel LogFromLevel { get; set; }
        public void Log(LogLevel logLevel, string message)
        {
            SetConsoleColor(logLevel);
            if (logLevel  >= LogFromLevel)
            {
                Console.WriteLine(DateTime.Now + ": " + logLevel.ToString() + " : " + message);
            }

            Console.ForegroundColor = ConsoleColor.Black;
        }

        public void Log(LogLevel logLevel, string source, string message)
        {
            SetConsoleColor(logLevel);
            Console.WriteLine(DateTime.Now + ": " + logLevel.ToString() + " : " + source + ":" + message);
            Console.ForegroundColor = ConsoleColor.Black;
        }

        private void SetConsoleColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case LogLevel.Information :
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

            }
        }
    }
}
