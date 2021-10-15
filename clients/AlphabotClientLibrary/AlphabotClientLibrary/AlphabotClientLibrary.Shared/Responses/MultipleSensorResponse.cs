using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class MultipleSensorResponse : IAlphabotResponse
    {
        public List<IAlphabotResponse> SensorResponses { get; private set; }

        public MultipleSensorResponse(List<IAlphabotResponse> sensorResponses)
        {
            SensorResponses = sensorResponses;
        }
    }
}
