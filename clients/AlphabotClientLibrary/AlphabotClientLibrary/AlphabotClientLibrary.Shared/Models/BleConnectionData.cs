using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
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
