using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            byte[] ret = new byte[5];

            byte[] anchorId = { _anchorId };

            byte[] positionData = _position.ToByteArray();

            ret = anchorId.Concat(positionData).ToArray();

            return ret;
        }
    }
}
