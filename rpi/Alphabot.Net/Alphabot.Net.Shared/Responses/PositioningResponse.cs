using System;
using AlphabotClientLibrary.Shared.Contracts;

namespace Alphabot.Net.Shared.Responses
{
    public class PositioningResponse : IAlphabotResponse
    {
        public Position Position { get; private set; }

        public PositioningResponse(Position position)
        {
            Position = position;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = { 0x02, Position.ToByteArray()[0], Position.ToByteArray()[1], Position.ToByteArray()[2], Position.ToByteArray()[3] };
            return bytes;
        }
    }
}
