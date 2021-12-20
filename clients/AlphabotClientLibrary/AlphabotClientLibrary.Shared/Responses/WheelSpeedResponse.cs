using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class WheelSpeedResponse : IAlphabotResponse
    {
        /// <summary>
        /// Speed in meters per second
        /// </summary>
        public sbyte Speed { get; private set; }

        public WheelSpeedResponse(sbyte speed)
        {
            Speed = speed;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.WheelSpeed;
        }
    }
}
