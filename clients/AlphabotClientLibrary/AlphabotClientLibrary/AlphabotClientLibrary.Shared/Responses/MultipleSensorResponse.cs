using System;
using System.Collections.Generic;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class MultipleSensorResponse : IAlphabotResponse
    {
        public enum SensorType
        {
            None,
            DistanceSensor,
            Positioning,
            Compass
        }

        public List<IAlphabotResponse> SensorResponses { get; private set; }

        public MultipleSensorResponse(List<IAlphabotResponse> sensorResponses)
        {
            SensorResponses = sensorResponses;
        }
    }
}
