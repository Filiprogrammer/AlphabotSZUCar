using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class RemoveObstacleRequest : IAlphabotRequest
    {
        private Position _position;
        private ushort _id;

        public RemoveObstacleRequest(Obstacle obstacle)
        {
            if (obstacle.Id == 65535)
                _position = obstacle.Position;
            else
                _id = obstacle.Id;
        }

        /// <summary>
        /// Remove obstacle by its position. If there are multiple objects in the same position, all of them will be removed.
        /// </summary>
        /// <param name="position"></param>
        public RemoveObstacleRequest(Position position)
        {
            _position = position;
        }

        /// <summary>
        /// Remove obstacle by its id. If there are multiple objects in the same position, all of them will be removed.
        /// </summary>
        /// <param name="position"></param>
        public RemoveObstacleRequest(ushort Id)
        {
            _id = Id;
        }

        public BleInformation GetBleInformation()
        {
            byte[] timebytes = GetMillisecondsSinceEpoch();
            byte[] bytes;

            // If position is null, the obstacle will be removed by its id.
            if (_position == null)
                bytes = timebytes.Concat(BitConverter.GetBytes(_id)).ToArray();
            // Else it will be removed by its position.
            else
                bytes = timebytes.Concat(_position.ToByteArray()).ToArray();

            return new BleInformation(BleUuids.REMOVE_OBSTACLE, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] ret;

            // If position is null, the obstacle will be removed by its id.
            if (_position == null)
            {
                ret = new byte[3];
                ret[0] = 0x0A; // Packet ID 0x0A
                byte[] idInBytes = BitConverter.GetBytes(_id);
                ret[1] = idInBytes[0];
                ret[2] = idInBytes[1];
            }
            // Else it will be removed by its position.
            else
            {
                byte[] packetId = { 0x0B };
                byte[] positionData = _position.ToByteArray();
                ret = packetId.Concat(positionData).ToArray();
            }

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
