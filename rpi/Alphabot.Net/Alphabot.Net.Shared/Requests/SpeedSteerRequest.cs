using Alphabot.Net.Shared.Contracts;
using System;

namespace Alphabot.Net.Shared.Requests
{
    public class SpeedSteerRequest : IAlphabotRequest
    {
        public sbyte _steer;
        public sbyte _speed;

        public SpeedSteerRequest(sbyte steer, sbyte speed)
        {
            _steer = steer;
            _speed = speed;
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[3];

            ret[0] = 0x01; // Packet ID 0x01

            ret[1] = (byte) _speed;
            ret[2] = (byte) _steer;

            return ret;
        }
    }
}
