﻿using System;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public abstract class DistanceSensorResponse : IAlphabotResponse
    {
        public short Degree { get; private set; }

        public ushort Distance { get; private set; }

        public DistanceSensorResponse(short degree, ushort distance)
        {
            Degree = degree;
            Distance = distance;
        }

        public abstract AlphabotResponseType GetResponseType();
    }
}
