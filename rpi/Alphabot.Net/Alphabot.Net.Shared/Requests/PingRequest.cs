using System;
using System.Linq;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Responses;

namespace Alphabot.Net.Shared.Requests
{
    public class PingRequest : IAlphabotRequest
    {
        private byte[] _dataBytes;

        public PingRequest(byte[] dataBytes)
        {
            _dataBytes = dataBytes;
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[9];
            ret[0] = 0x06;
            Array.Copy(_dataBytes, 0, ret, 1, 8);
            return ret;
        }

        public PingResponse GetPingResponse()
        {
            return new PingResponse(_dataBytes);
        }
    }
}
