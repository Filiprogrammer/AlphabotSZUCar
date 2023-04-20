﻿using System;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class BackDistanceSensorResponse : DistanceSensorResponse
    {
        public BackDistanceSensorResponse(short degree, ushort distance) : base(degree, distance) {}

        public override AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.BackDistanceSensor;
        }
    }
}