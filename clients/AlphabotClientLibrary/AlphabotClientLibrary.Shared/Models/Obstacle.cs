using System;

namespace AlphabotClientLibrary.Shared.Models
{
    public class Obstacle
    {
        /// <summary>
        /// Id of the obstacle, returns 65.535 if the Id is not set in constructor
        /// </summary>
        public ushort Id { get; private set; }

        public Position Position { get; private set; }

        /// <summary>
        /// Obstacle width in centimetres
        /// </summary>
        public ushort Width { get; private set; }

        /// <summary>
        /// Obstacle height in centimetres
        /// </summary>
        public ushort Height { get; private set; }

        /// <summary>
        /// Creates an Obstacle object with unknown Id. The Id gets set to 65.535 
        /// </summary>
        /// <param name="position">Position of the obstacle</param>
        /// <param name="width">Obstacle width in centimetres</param>
        /// <param name="height">Obstacle height in centimetres</param>
        public Obstacle(Position position, ushort width, ushort height)
        {
            Position = position;
            Width = width;
            Height = height;
            Id = 65535;
        }

        /// <summary>
        /// Creates an Obstacle object with known Id.
        /// </summary>
        /// <param name="position">Position of the obstacle</param>
        /// <param name="width">Obstacle width in centimetres</param>
        /// <param name="height">Obstacle height in centimetres</param>
        /// <param name="id">Obstacle Id, must not be 65.535</param>
        public Obstacle(Position position, ushort width, ushort height, ushort id)
        {
            Position = position;
            Width = width;
            Height = height;
            Id = id;
        }

        /// <summary>
        /// Creates an Obstacle object out of bytes. Uses the following byte order:
        /// byte[0-1]: PositionX (short), byte[2-3]: PositionY (short), byte[4-5] Width (ushort), byte[6-7] Height (ushort), byte [8-9] Id (ushort)
        /// </summary>
        /// <param name="bytes"></param>
        public Obstacle(byte[] bytes)
        {
            byte[] positionBytes = new byte[4];
            byte[] widthBytes = new byte[2];
            byte[] heightBytes = new byte[2];
            byte[] idBytes = new byte[2];

            Array.Copy(bytes, 0, positionBytes, 0, 4);
            Array.Copy(bytes, 4, widthBytes, 0, 2);
            Array.Copy(bytes, 6, heightBytes, 0, 2);
            Array.Copy(bytes, 8, idBytes, 0, 2);

            Position = new Position(positionBytes);
            Width = (ushort)(widthBytes[0] | (widthBytes[1] << 8));
            Height = (ushort)(heightBytes[0] | (heightBytes[1] << 8));
            Id = (ushort)(idBytes[0] | (idBytes[1] << 8));
        }

        /// <summary>
        /// Converts the obstacle data into a byte array. If the Id was not set in the constructor, the array ends after byte[7] without the Id
        /// </summary>
        /// <returns>byte[0-1]: PositionX (short), byte[2-3]: PositionY (short), byte[4-5] Width (ushort), byte[6-7] Height (ushort), (optional) byte [8-9] Id (ushort)</returns>
        public byte[] ToByteArray()
        {
            byte[] ret;

            // If the id is 65535, it was not set in constructor and will not be added to the array.
            if (Id == 65535)
                ret = new byte[8];
            else
                ret = new byte[10];

            byte[] posData = Position.ToByteArray();

            byte[] widthInBytes = BitConverter.GetBytes(Width);
            byte[] heightInBytes = BitConverter.GetBytes(Height);

            ret[0] = posData[0];
            ret[1] = posData[1];
            ret[2] = posData[2];
            ret[3] = posData[3];
            ret[4] = widthInBytes[0];
            ret[5] = widthInBytes[1];
            ret[6] = heightInBytes[0];
            ret[7] = heightInBytes[1];

            if (Id != 65535)
            {
                byte[] idInBytes = BitConverter.GetBytes(Id);
                ret[8] = idInBytes[0];
                ret[9] = idInBytes[1];
            }

            return ret;
        }
    }
}
