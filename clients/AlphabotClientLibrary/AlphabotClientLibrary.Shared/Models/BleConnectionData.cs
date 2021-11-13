using System;
using System.Net.NetworkInformation;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Models
{
    public class BleConnectionData : IConnectionData
    {
        public PhysicalAddress MacAddress { get; private set; }

        public BleConnectionData(PhysicalAddress macAddress)
        {
            MacAddress = macAddress;
        }
    }
}
