using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class CalibrateSteeringRequest : IAlphabotRequest
    {
        public BleInformation GetBleInformation()
        {
            byte[] bytes = new byte[1];
            bytes[0] = 0x01; // Start steering calibration

            return new BleInformation(BleUuids.CALIBRATE, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x03 };

            return packetId;
        }
    }
}
