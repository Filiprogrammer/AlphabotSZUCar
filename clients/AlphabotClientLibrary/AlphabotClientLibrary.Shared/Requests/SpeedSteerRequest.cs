using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class SpeedSteerRequest : IAlphabotRequest
    {
        private sbyte _speed;
        private sbyte _steer;

        public SpeedSteerRequest(sbyte speed, sbyte steer)
        {
            _speed = speed;
            _steer = steer;
        }

        public BleInformation GetBleInformation()
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)_speed;
            bytes[1] = (byte)_steer;

            return new BleInformation(BleUuids.DRIVE_STEER, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[3];
            ret[0] = 0x01; // Packet ID 0x01
            ret[1] = (byte)_speed;
            ret[2] = (byte)_steer;

            return ret;
        }
    }
}
