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

        private List<IAlphabotResponse> _sensorResponses = new List<IAlphabotResponse>();

        public IReadOnlyCollection<IAlphabotResponse> SensorResponses {
            get { return _sensorResponses; }
        }

        public MultipleSensorResponse(List<IAlphabotResponse> sensorResponses)
        {
            _sensorResponses = sensorResponses;
        }
    }
}
