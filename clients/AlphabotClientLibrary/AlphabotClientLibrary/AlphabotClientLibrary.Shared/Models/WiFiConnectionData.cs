using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
