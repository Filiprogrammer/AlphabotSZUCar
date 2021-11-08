using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class ConfigurePositioningAnchorRequest : IAlphabotRequest
    {
        byte _anchorId;
        Position _position;

        public ConfigurePositioningAnchorRequest(byte anchorId, Position position)
        {
            _anchorId = anchorId;
            _position = position;
        }

        public BleInformation GetBleInformation()
        {
            //aktuell keine Ble Implementation/Dokumentation?

            throw new NotImplementedException();
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x07 }; //packet id 0x07

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
