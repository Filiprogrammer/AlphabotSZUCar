using System;
using System.Collections;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class ToggleResponse : IAlphabotResponse
    {
        #region properties
        public bool DoCompassCalibration { get; private set; }
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
        public bool LogAccelerometer { get; private set; }
        public bool LogGyroscope { get; private set; }
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

            LogGyroscope = bitArray[8];
            LogAccelerometer = bitArray[9];
            LogWheelSpeed = bitArray[10];
            LogAnchorDistances = bitArray[11];
            LogCompassDirection = bitArray[12];
            LogPathfinderPath = bitArray[13];
            LogObstacleDistance = bitArray[14];
            LogPositioning = bitArray[15];

            //bit 0 and 1 are not used

            DoExploreMode = bitArray[2];
            DoNavigationMode = bitArray[3];
            DoCollisionAvoidance = bitArray[4];
            DoPositioningSystem = bitArray[5];
            DoInvite = bitArray[6];
            DoCompassCalibration = bitArray[7];
        }
    }
}
