using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Responses;
using System;
using System.Linq;

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
            return new byte[0];
        }

        public PingResponse GetPingResponse()
        {
            return new PingResponse(_dataBytes);
        }
    }
}
