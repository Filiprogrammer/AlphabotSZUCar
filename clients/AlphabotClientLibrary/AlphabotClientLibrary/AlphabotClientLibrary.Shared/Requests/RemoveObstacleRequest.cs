using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class RemoveObstacleRequest : IAlphabotRequest
    {
        Position _position;
        ushort _id;

        public RemoveObstacleRequest(Obstacle obstacle)
        {
            if (obstacle.Id == 65535)
            {
                _position = obstacle.Position;
            }
            else
            {
                _id = obstacle.Id;
            }
        }

        /// <summary>
        /// Remove obstacle by position. If there are multiple objects on the same position, all of them will be removed.
        /// </summary>
        /// <param name="position"></param>
        public RemoveObstacleRequest(Position position)
        {
            _position = position;
        }

        public RemoveObstacleRequest(ushort Id)
        {
            _id = Id;
        }

        public BleInformation GetBleInformation()
        {
            Guid uuid = new Guid("6d43e0df-682b-45ef-abb7-814ecf475771");
            byte[] bytes;

            //if position is null, the obstacle will be removed by its id
            if (_position == null)
            {
                bytes = new byte[2];

                byte[] idInBytes = BitConverter.GetBytes(_id);
                bytes[0] = idInBytes[0];
                bytes[1] = idInBytes[1];
            }
            //else it will be removed by its position
            else
            {
                bytes = _position.ToByteArray();
            }

            return new BleInformation(uuid, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] ret;

            //if position is null, the obstacle will be removed by its id
            if (_position == null)
            {
                ret = new byte[3];

                ret[0] = 0x0A; //packet Id 0x0A
                byte[] idInBytes = BitConverter.GetBytes(_id);
                ret[1] = idInBytes[0];
                ret[2] = idInBytes[1];
            }
            //else it will be removed by its position
            else
            {
                byte[] packetId = { 0x0B };
                byte[] positionData = _position.ToByteArray();

                ret = packetId.Concat(positionData).ToArray();
            }

            return ret;
        }
    }
}
