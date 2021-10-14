using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabotClientLibrary.Shared.Models
{
    public class Obstacle
    {
        /// <summary>
        /// returns 65535 if the Id is not set in constructor
        /// </summary>
        public ushort Id { get; private set; }
        public Position Position { get; private set; }
        public ushort Width { get; private set; }
        public ushort Height { get; private set; }

        public Obstacle(Position position, ushort width, ushort height)
        {
            Position = position;
            Width = width;
            Height = height;
            Id = 65535;
        }

        public Obstacle(Position position, ushort width, ushort height, ushort id)
        {
            Position = position;
            Width = width;
            Height = height;
            Id = id;
        }

        /// <summary>
        /// Converts the obstacle data into a byte array. If the Id was not set in the constructor, the array ends after byte[7] without the Id
        /// </summary>
        /// <returns>byte[0-1]: PositionX (short), byte[2-3]: PositionY (short), byte[4-5] Width (ushort), byte[6-7] Height (ushort), (optional) byte [8-9] Id (ushort)</returns>
        public byte[] ToByteArray()
        {
            byte[] ret;

            //if the id is 65535, it was not set in constructor and will not be added in array
            if (Id == 65535)
            {
                ret = new byte[8];
            }
            else
            {
                ret = new byte[10];
            }

            byte[] posData = Position.ToByteArray();

            byte[] widthInBytes = BitConverter.GetBytes(Width);
            byte[] heightInBytes = BitConverter.GetBytes(Height);

            ret[0] = posData[0];
            ret[1] = posData[1];
            ret[2] = posData[2];
            ret[3] = posData[3];
            ret[4] = widthInBytes[1];
            ret[5] = widthInBytes[0];
            ret[6] = heightInBytes[1];
            ret[7] = heightInBytes[0];

            if(Id != 65535)
            {
                byte[] idInBytes = BitConverter.GetBytes(Id);
                ret[8] = idInBytes[1];
                ret[9] = idInBytes[0];
            }

            return ret;
        }
    }
}
