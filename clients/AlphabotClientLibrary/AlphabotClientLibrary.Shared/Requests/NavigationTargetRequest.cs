using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class NavigationTargetRequest : IAlphabotRequest
    {
        private Position _position;

        public NavigationTargetRequest(Position position)
        {
            _position = position;
        }

        public BleInformation GetBleInformation()
        {
            byte[] posBytes = _position.ToByteArray();
            byte[] timestamp = GetMillisecondsSinceEpoch();
            byte[] data = posBytes.Concat(timestamp).ToArray();

            return new BleInformation(BleUuids.NAVIGATION_TARGET, data);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x08 };
            byte[] positionData = _position.ToByteArray();

            return packetId.Concat(positionData).ToArray();
        }
        private byte[] GetMillisecondsSinceEpoch()
        {
            long millisecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            byte[] timeInBytes = BitConverter.GetBytes(millisecondsSinceEpoch);

            return timeInBytes;
        }
    }
}
