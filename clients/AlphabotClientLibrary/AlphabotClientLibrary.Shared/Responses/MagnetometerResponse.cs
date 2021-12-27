using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class MagnetometerResponse : IAlphabotResponse
    {
        /// <summary>
        /// The magnetic flux density in µT
        /// </summary>
        public float XAxis { get; private set; }

        /// <summary>
        /// The magnetic flux density in µT
        /// </summary>
        public float YAxis { get; private set; }
        /// <summary>
        /// The magnetic flux density in µT
        /// </summary>
        public float ZAxis { get; private set; }

        public MagnetometerResponse(float xAxis, float yAxis, float zAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.Magnetometer;
        }
    }
}
