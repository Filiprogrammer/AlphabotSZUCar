using System;
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

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.DistanceSensor;
        }
    }
}
