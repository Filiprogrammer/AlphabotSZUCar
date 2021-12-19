using Alphabot.Net.Shared.Contracts;
using System;

namespace Alphabot.Net.Shared.Requests
{
    public class CalibrateSteeringRequest : IAlphabotRequest
    {
        public byte[] GetBytes()
        {
            byte[] packetId = { 0x03 };

            return packetId;
        }
    }
}
