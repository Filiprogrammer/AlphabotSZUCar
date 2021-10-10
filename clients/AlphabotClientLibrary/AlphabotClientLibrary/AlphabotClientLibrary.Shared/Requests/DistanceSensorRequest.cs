using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class DistanceSensorRequest : IAlphabotRequest
    {
        short _degree;

        public DistanceSensorRequest(short degree)
        {
            if(degree < 0 || degree > 359)
            {
                throw new ArgumentOutOfRangeException("Degree must be between 0 and 359");
            }

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
            byte[] shortInBytes = BitConverter.GetBytes(_degree);

            ret[0] = 0x02; //Packet ID 0x02

            ret[1] = shortInBytes[0];
            ret[2] = shortInBytes[1];

            return ret;
        }
    }
}
