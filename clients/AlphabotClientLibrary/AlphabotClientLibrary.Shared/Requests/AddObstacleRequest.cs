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
            byte[] obsData = _obstacle.ToByteArray();
            byte[] timestamp = GetMillisecondsSinceEpoch();
            byte[] data = timestamp.Concat(obsData).ToArray();

            return new BleInformation(BleUuids.ADD_OBSTACLE, data);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x09 };
            byte[] obstacleData = _obstacle.ToByteArray();

            return packetId.Concat(obstacleData).ToArray();
        }

        private byte[] GetMillisecondsSinceEpoch()
        {
            long millisecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            byte[] timeInBytes = BitConverter.GetBytes(millisecondsSinceEpoch);

            return timeInBytes;
        }
    }
}
