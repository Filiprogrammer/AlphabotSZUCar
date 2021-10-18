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
            byte[] packetId = { 0x06 };

            byte[] time = GetMillisecondsSinceEpoch();

            return packetId.Concat(time).ToArray();
        }

        private byte[] GetMillisecondsSinceEpoch()
        {
            long millisecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            byte[] timeInBytes = {
                (byte)millisecondsSinceEpoch,
                (byte)(millisecondsSinceEpoch >> 8),
                (byte)(millisecondsSinceEpoch >> 16),
                (byte)(millisecondsSinceEpoch >> 24),
                (byte)(millisecondsSinceEpoch >> 32),
                (byte)(millisecondsSinceEpoch >> 40),
                (byte)(millisecondsSinceEpoch >> 48),
                (byte)(millisecondsSinceEpoch >> 56)
            };

            return timeInBytes;
        }
    }
}
