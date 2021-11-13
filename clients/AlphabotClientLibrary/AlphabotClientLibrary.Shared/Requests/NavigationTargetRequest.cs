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
            return new BleInformation(BleUuids.NAVIGATION_TARGET, _position.ToByteArray());
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x08 };
            byte[] positionData = _position.ToByteArray();

            return packetId.Concat(positionData).ToArray();
        }
    }
}
