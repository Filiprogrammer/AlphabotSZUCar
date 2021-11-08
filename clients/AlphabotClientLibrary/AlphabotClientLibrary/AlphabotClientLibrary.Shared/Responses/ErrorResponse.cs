using System;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class ErrorResponse : IAlphabotResponse
    {
        public ErrorType Error { get; private set; }
        public byte Header { get; private set; }
        public byte[] Payload { get; private set; }

        public enum ErrorType
        {
            UnknownError = 0x00,
            UnknownProtocol = 0x01,
            NotSupportedProtocol = 0x02,
            UnknownPacketId = 0x03,
            WrongPayload = 0x04
        }

        public ErrorResponse(ErrorType errorType, byte[] packet)
        {
            Payload = new byte[packet.Length - 1];
            Header = packet[0];
            Array.Copy(packet, 1, Payload, 0, packet.Length - 1);
            Error = errorType;
        }
    }
}
