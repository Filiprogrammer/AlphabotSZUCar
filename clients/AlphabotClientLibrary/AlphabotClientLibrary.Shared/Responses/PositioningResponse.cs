using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class PositioningResponse : IAlphabotResponse
    {
        public Position Position { get; private set; }

        public PositioningResponse(Position position)
        {
            Position = position;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.Positioning;
        }
    }
}
