using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class DistanceSensorRequest : IAlphabotRequest
    {
        private short _degree;

        public DistanceSensorRequest(short degree)
        {
            if(degree < 0 || degree > 359)
                throw new ArgumentOutOfRangeException("Degree is outside the range of 0 and 359");

            _degree = degree;
        }
        public BleInformation GetBleInformation()
        {
            //Es gibt im Protokoll keine DistanceSensorRequest für Ble?
            throw new NotImplementedException();
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[3];
            ret[0] = 0x02; // Packet ID 0x02
            ret[1] = (byte)_degree;
            ret[2] = (byte)(_degree >> 8);

            return ret;
        }
    }
}
