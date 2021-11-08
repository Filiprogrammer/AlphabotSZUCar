using System;
using System.Net;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Models
{
    public class WiFiConnectionData : IConnectionData
    {
        public IPEndPoint IPEndPoint { get; private set; }

        public WiFiConnectionData(IPEndPoint ipEndPoint)
        {
            IPEndPoint = ipEndPoint;
        }

        public WiFiConnectionData(IPAddress ipaddress, ushort port)
        {
            IPEndPoint = new IPEndPoint(ipaddress, port);
        }
    }
}
