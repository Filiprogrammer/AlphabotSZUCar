using Alphabot.Net.Shared.Contracts;
using System;
using System.Linq;

namespace Alphabot.Net.Shared.Requests
{
    public class ConfigurePositioningAnchorRequest : IAlphabotRequest
    {
        public byte AnchorId { get; private set; }
        public Position Position { get; private set; }

        public ConfigurePositioningAnchorRequest(byte anchorId, Position position)
        {
            if (anchorId < 0 || anchorId > 2)
                throw new ArgumentException("AnchorID must be between 0 and 2");

            AnchorId = anchorId;
            Position = position;
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x07 }; // Packet ID 0x07
            byte[] data = GetDataBytes();

            return packetId.Concat(data).ToArray();
        }

        private byte[] GetDataBytes()
        {
            byte[] anchorId = { AnchorId };
            byte[] positionData = Position.ToByteArray();

            return anchorId.Concat(positionData).ToArray();
        }
    }
}
