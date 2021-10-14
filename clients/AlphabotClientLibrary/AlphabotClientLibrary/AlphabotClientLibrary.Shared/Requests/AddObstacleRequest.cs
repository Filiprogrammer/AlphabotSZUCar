using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class AddObstacleRequest : IAlphabotRequest
    {
        Obstacle _obstacle;
        public AddObstacleRequest(Obstacle obstacle)
        {
            _obstacle = obstacle;
        }

        public BleInformation GetBleInformation()
        {
            Guid uuid = new Guid("60db37c7-afeb-4d40-bb17-a19a07d6fc95");
            byte[] bytes = _obstacle.ToByteArray();

            return new BleInformation(uuid, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x09 };

            byte[] obstacleData = _obstacle.ToByteArray();

            return packetId.Concat(obstacleData).ToArray();
        }
    }
}
