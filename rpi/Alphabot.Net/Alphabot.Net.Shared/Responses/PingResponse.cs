using AlphabotClientLibrary.Shared.Contracts;
using System;

namespace Alphabot.Net.Shared.Responses
{
    public class PingResponse : IAlphabotResponse
    {
        private byte[] _dataBytes;

        public PingResponse(byte[] dataBytes)
        {
            _dataBytes = dataBytes;
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[9];
            ret[0] = 0x05;
            Array.Copy(_dataBytes, 0, ret, 1, 8);
            return ret;
        }
    }
}
