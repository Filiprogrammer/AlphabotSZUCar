using System;
using System.Linq;
using AlphabotClientLibrary.Shared.Contracts;

namespace Alphabot.Net.Shared.Responses
{
    public class ErrorResponse : IAlphabotResponse
    {
        public enum ErrorType
        {
            UnknownError = 0x00,
            UnknownProtocol = 0x01,
            NotSupportedProtocol = 0x02,
            UnknownPacketId = 0x03,
            WrongPayload = 0x04
        }

        public ErrorType Error { get; private set; }

        public byte Header { get; private set; }

        public byte[] Payload { get; private set; }

        public ErrorResponse(ErrorType errorType, byte[] packet)
        {
            Payload = new byte[packet.Length - 1];
            Header = packet[0];
            Array.Copy(packet, 1, Payload, 0, packet.Length - 1);
            Error = errorType;
        }

        public byte[] GetBytes()
        {
            byte[] ret;
            byte[] firstThreeBytes = { 0x0C, (byte)Error, Header };

            ret = firstThreeBytes.Concat(Payload).ToArray();
            return ret;
        }
    }
}
