using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class GyroscopeResponse : IAlphabotResponse
    {
        /// <summary>
        /// The speed in degree per second
        /// </summary>

        public float XAxis { get; private set; }
        /// <summary>
        /// The speed in degree per second
        /// </summary>
        public float YAxis { get; private set; }
        /// <summary>
        /// The speed in degree per second
        /// </summary>
        public float ZAxis { get; private set; }

        public GyroscopeResponse(float xAxis, float yAxis, float zAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.Gyroscope;
        }
    }
}
