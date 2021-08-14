using System;
using System.Collections.Generic;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Remote.Contracts;

namespace Alphabot.Net.Remote.Core
{
    public class ProtocolParser: IProtocolParser
    {
        private readonly Prefs _prefs = Prefs.GetInstance();

        public ProtocolParser(string request)
        {
            Request = request.Trim().ToLower();
        }

        public string Request { get; private set; }

        public string GetCommand()
        {
            string command;
            var seperator = _prefs.ServiceSettings.CommandParamSeparator;
            if (Request.IndexOf(seperator) <= 0) // no args available 
            {
                command = Request.ToLower().Trim();
            }
            else
            {
                command = Request.Substring(0, Request.IndexOf(seperator)).Trim();
            }

            return command;
        }

        public string[] GetArgs()
        {
            var args = new List<string>();
            args.AddRange(ParseArgs(Request));
            return args.ToArray();
        }



        private string[] ParseArgs(string commandLine)
        {
            List<string> args = new List<string>();
            string[] result = commandLine.Split(_prefs.ServiceSettings.CommandParamSeparator);
            if (result.Length > 1) //  args available
            {
                for (int i = 1; i < result.Length; i++)
                {
                    args.Add(result[i].Trim());
                }
            }

            return args.ToArray();
        }
        
    }
}
