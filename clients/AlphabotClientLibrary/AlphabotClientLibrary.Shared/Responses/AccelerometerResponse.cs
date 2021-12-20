using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class AccelerometerResponse : IAlphabotResponse
    {
        public float XAxis { get; private set; }
        public float YAxis { get; private set; }
        public float ZAxis { get; private set; }

        public AccelerometerResponse(float xAxis, float yAxis, float zAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.Accelerometer;
        }
    }
}
