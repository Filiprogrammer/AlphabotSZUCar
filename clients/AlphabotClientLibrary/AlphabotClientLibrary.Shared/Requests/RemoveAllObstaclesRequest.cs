using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class RemoveAllObstaclesRequest : IAlphabotRequest
    {
        public BleInformation GetBleInformation()
        {
            byte[] bytes = new byte[0];

            return new BleInformation(BleUuids.REMOVE_OBSTACLE, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x0C };

            return packetId;
        }
    }
}
