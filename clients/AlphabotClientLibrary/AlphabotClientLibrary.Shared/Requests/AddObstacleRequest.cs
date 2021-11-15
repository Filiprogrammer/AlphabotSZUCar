using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class AddObstacleRequest : IAlphabotRequest
    {
        private Obstacle _obstacle;

        public AddObstacleRequest(Obstacle obstacle)
        {
            _obstacle = obstacle;
        }

        public BleInformation GetBleInformation()
        {
            return new BleInformation(BleUuids.ADD_OBSTACLE, _obstacle.ToByteArray());
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x09 };
            byte[] obstacleData = _obstacle.ToByteArray();

            return packetId.Concat(obstacleData).ToArray();
        }
    }
}
