using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
