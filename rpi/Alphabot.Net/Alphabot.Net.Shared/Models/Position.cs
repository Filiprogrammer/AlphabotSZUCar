using System;

namespace Alphabot.Net.Shared
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

        public Position(byte[] bytes)
        {
            if (bytes.Length != 4)
                throw new ArgumentException("The array must consist of 4 elements");

            PositionX = (short)(bytes[0] | (bytes[1] << 8));
            PositionY = (short)(bytes[2] | (bytes[3] << 8));
        }

        /// <summary>
        /// Converts the position in a byte array, 
        /// </summary>
        /// <returns>byte[0-1]: PositionX (short), byte[2-3]: PositionY (short)</returns>
        public byte[] ToByteArray()
        {
            byte[] distanceXInBytes = BitConverter.GetBytes(PositionX);
            byte[] distanceYInBytes = BitConverter.GetBytes(PositionY);

            byte[] ret = new byte[4];
            ret[0] = distanceXInBytes[0];
            ret[1] = distanceXInBytes[1];
            ret[2] = distanceYInBytes[0];
            ret[3] = distanceYInBytes[1];

            return ret;
        }
    }
}
