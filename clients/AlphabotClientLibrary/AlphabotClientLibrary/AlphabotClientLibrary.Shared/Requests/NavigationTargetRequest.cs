using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class NavigationTargetRequest : IAlphabotRequest
    {
        Position _position;

        public NavigationTargetRequest(Position position)
        {
            _position = position;
        }

        public BleInformation GetBleInformation()
        {
            Guid uuid = new Guid("f56f0a15-52ae-4ad5-bfe1-557eed983618");
            byte[] bytes = _position.ToByteArray();

            return new BleInformation(uuid, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x08 };
            byte[] positionData = _position.ToByteArray();

            return packetId.Concat(positionData).ToArray();
        }
    }
}
