using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class PingRequest : IAlphabotRequest
    {
        public BleInformation GetBleInformation()
        {
            return new BleInformation(BleUuids.PINGCLIENT, GetMillisecondsSinceEpoch());
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
            byte[] timeInBytes = BitConverter.GetBytes(millisecondsSinceEpoch);

            return timeInBytes;
        }
    }
}
