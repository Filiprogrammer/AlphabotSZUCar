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

        public WiFiConnectionData(string address, ushort port)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(address);

            if(addresses.Length < 1)
                throw new WebException("The address resolver could not resolve the host name.", WebExceptionStatus.NameResolutionFailure);

            IPEndPoint = new IPEndPoint(addresses[0], port);
        }
    }
}
