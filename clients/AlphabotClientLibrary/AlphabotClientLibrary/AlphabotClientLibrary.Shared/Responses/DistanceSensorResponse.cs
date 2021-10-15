using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class DistanceSensorResponse : IAlphabotResponse
    {
        public short Degree { get; private set; }
        public ushort Distance { get; private set; }

        public DistanceSensorResponse(short degree, ushort distance)
        {
            Degree = degree;
            Distance = distance;
        }
    }
}
