using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class CompassResponse : IAlphabotResponse
    {
        public short Degree { get; private set; }

        public CompassResponse(short degree)
        {
            Degree = degree;
        }
    }
}
