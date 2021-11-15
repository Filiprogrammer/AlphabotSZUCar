using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class ConfigurePositioningAnchorRequest : IAlphabotRequest
    {
        private byte _anchorId;
        private Position _position;

        public ConfigurePositioningAnchorRequest(byte anchorId, Position position)
        {
            if(anchorId < 0 || anchorId > 2)
            {
                throw new ArgumentException("AnchorID must be between 0 and 2");
            }

            _anchorId = anchorId;
            _position = position;
        }

        public BleInformation GetBleInformation()
        {
            byte[] bytes = new byte[12];
            byte[] posBytes = _position.ToByteArray();

            switch (_anchorId)
            {
                case 0:
                    bytes[0] = posBytes[0];
                    bytes[1] = posBytes[1];
                    bytes[2] = posBytes[2];
                    bytes[3] = posBytes[3];
                    break;
                case 1:
                    bytes[4] = posBytes[0];
                    bytes[5] = posBytes[1];
                    bytes[6] = posBytes[2];
                    bytes[7] = posBytes[3];
                    break;
                case 2:
                    bytes[8] = posBytes[0];
                    bytes[9] = posBytes[1];
                    bytes[10] = posBytes[2];
                    bytes[11] = posBytes[3];
                    break;
            }

            return new BleInformation(BleUuids.ANCHOR_LOCATIONS, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x07 }; // Packet ID 0x07
            byte[] data = GetDataBytes();

            return packetId.Concat(data).ToArray();
        }

        private byte[] GetDataBytes()
        {
            byte[] anchorId = { _anchorId };
            byte[] positionData = _position.ToByteArray();

            return anchorId.Concat(positionData).ToArray();
        }
    }
}
