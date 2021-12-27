using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class AccelerometerResponse : IAlphabotResponse
    {
        /// <summary>
        /// The acceleration in m/s^2
        /// </summary>
        public float XAxis { get; private set; }
        /// <summary>
        /// The acceleration in m/s^2
        /// </summary>
        public float YAxis { get; private set; }
        /// <summary>
        /// The acceleration in m/s²
        /// </summary>
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
