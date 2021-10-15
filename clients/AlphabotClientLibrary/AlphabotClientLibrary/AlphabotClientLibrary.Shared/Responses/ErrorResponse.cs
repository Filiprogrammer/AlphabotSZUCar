using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            unknownError = 0x00,
            unknownProtocol = 0x01,
            notSupportedProtocol = 0x02,
            unknownPacketId = 0x03,
            wrongPayload = 0x04
        }

        public ErrorResponse(ErrorType errorType, byte[] packet)
        {
            Header = packet[0];

            Array.Copy(packet, 1, Payload, 0, packet.Length - 1);

            Error = errorType;
        }
    }
}
