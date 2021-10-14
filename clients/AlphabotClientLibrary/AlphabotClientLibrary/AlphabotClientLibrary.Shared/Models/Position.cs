using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabotClientLibrary.Shared.Models
{
    public class Position
    {
        /// <summary>
        /// X coordinate in centimetres
        /// </summary>
        public short PositionX { get; private set; }
        /// <summary>
        /// Y coordinate in centimetres
        /// </summary>
        public short PositionY { get; private set; }

        public Position(short x, short y)
        {
            PositionX = x;
            PositionY = y;
        }

        /// <summary>
        /// Converts the position in a byte array, 
        /// </summary>
        /// <returns>byte[0-1]: PositionX (short), byte[2-3]: PositionY (short)</returns>
        public byte[] ToByteArray()
        {
            byte[] ret = new byte[4];

            byte[] distanceXInBytes = BitConverter.GetBytes(PositionX);
            byte[] distanceYInBytes = BitConverter.GetBytes(PositionY);

            ret[0] = distanceXInBytes[1];
            ret[1] = distanceXInBytes[0];
            ret[2] = distanceYInBytes[1];
            ret[3] = distanceYInBytes[0];

            return ret;
        }
    }
}
