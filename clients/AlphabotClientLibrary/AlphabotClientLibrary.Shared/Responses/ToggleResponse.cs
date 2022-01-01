using System;
using System.Collections;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class ToggleResponse : IAlphabotResponse
    {
        #region properties
        public bool DoInvite { get; private set; }

        public bool DoPositioningSystem { get; private set; }

        public bool DoCollisionAvoidance { get; private set; }

        public bool DoNavigationMode { get; private set; }

        public bool DoExploreMode { get; private set; }

        public bool LogPositioning { get; private set; }

        public bool LogObstacleDistance { get; private set; }

        public bool LogPathfinderPath { get; private set; }

        public bool LogCompassDirection { get; private set; }

        public bool LogAnchorDistances { get; private set; }

        public bool LogWheelSpeed { get; private set; }

        public bool LogIMU { get; private set; }
        #endregion

        public ToggleResponse(ushort bitField)
        {
            byte[] bytes = BitConverter.GetBytes(bitField);
            SetValues(bytes);
        }

        public ToggleResponse(byte[] bytes)
        {
            SetValues(bytes);
        }

        private void SetValues(byte[] bytes)
        {
            BitArray bitArray = new BitArray(bytes);

            // Bits 0 to 2 are not used.
            DoExploreMode = bitArray[3];
            DoNavigationMode = bitArray[4];
            DoCollisionAvoidance = bitArray[5];
            DoPositioningSystem = bitArray[6];
            DoInvite = bitArray[7];

            // Bit 8 is not used.
            LogIMU = bitArray[9];
            LogWheelSpeed = bitArray[10];
            LogAnchorDistances = bitArray[11];
            LogCompassDirection = bitArray[12];
            LogPathfinderPath = bitArray[13];
            LogObstacleDistance = bitArray[14];
            LogPositioning = bitArray[15];
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.Toggle;
        }
    }
}
