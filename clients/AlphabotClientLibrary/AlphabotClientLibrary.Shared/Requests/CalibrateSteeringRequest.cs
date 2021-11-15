using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class CalibrateSteeringRequest : IAlphabotRequest
    {
        public BleInformation GetBleInformation()
        {
            byte[] bytes = new byte[1];
            bytes[0] = 0x00; // Start steering calibration
            byte[] time = GetMillisecondsSinceEpoch();
            byte[] data = bytes.Concat(time).ToArray();

            return new BleInformation(BleUuids.CALIBRATE, data);
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x03 };

            return packetId;
        }

        private byte[] GetMillisecondsSinceEpoch()
        {
            long millisecondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            byte[] timeInBytes = BitConverter.GetBytes(millisecondsSinceEpoch);

            return timeInBytes;
        }
    }
}
