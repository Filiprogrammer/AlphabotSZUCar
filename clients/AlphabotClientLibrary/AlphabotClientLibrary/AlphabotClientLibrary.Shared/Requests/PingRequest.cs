using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class PingRequest : IAlphabotRequest
    {
        public PingRequest() { }
        public BleInformation GetBleInformation()
        {
            Guid uuid = new Guid("117ad3a5-b257-4465-abd4-7dc12a4cf77d");
            byte[] bytes = GetMillisecondsSinceEpoch();

            return new BleInformation(uuid, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] ret;

            byte[] packetId = new byte[1];
            packetId[0] = 0x06; //Packet ID 0x06

            byte[] time = GetMillisecondsSinceEpoch();

            //combine packet id and time to one array
            ret = packetId.Concat(time).ToArray();

            return ret;
        }

        private byte[] GetMillisecondsSinceEpoch()
        {
            long millisecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            byte[] timeInBytes = BitConverter.GetBytes(millisecondsSinceEpoch);

            return timeInBytes;
        }
    }
}
