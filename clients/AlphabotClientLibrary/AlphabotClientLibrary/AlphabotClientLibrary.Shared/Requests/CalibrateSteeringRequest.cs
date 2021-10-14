using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class CalibrateSteeringRequest : IAlphabotRequest
    {
        public CalibrateSteeringRequest() { }

        public BleInformation GetBleInformation()
        {
            Guid uuid = new Guid("d39e8d54-8019-46c8-a977-db13871bac59");
            byte[] bytes = new byte[1];

            bytes[0] = 0x01; //start steering calibration

            return new BleInformation(uuid, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x03 };

            return packetId;
        }
    }
}
