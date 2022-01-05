using System;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class WheelSpeedResponse : IAlphabotResponse
    {
        /// <summary>
        /// Speed of the left wheel in meters per second
        /// </summary>
        public sbyte SpeedLeft { get; private set; }

        /// <summary>
        /// Speed of the right wheel in meters per second
        /// </summary>
        public sbyte SpeedRight { get; private set; }

        public WheelSpeedResponse(sbyte speedLeft, sbyte speedRight)
        {
            SpeedLeft = speedLeft;
            SpeedRight = speedRight;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.WheelSpeed;
        }
    }
}
