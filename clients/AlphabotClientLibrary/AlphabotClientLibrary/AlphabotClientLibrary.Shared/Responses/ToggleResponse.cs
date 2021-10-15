using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

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

            LogGyroscope = bitArray[0];
            LogAccelerometer = bitArray[1];
            LogWheelSpeed = bitArray[2];
            LogAnchorDistances = bitArray[3];
            LogCompassDirection = bitArray[4];
            LogPathfinderPath = bitArray[5];
            LogObstacleDistance = bitArray[6];
            LogPositioning = bitArray[7];

            //bit 8 and 9 are not used

            DoExploreMode = bitArray[10];
            DoNavigationMode = bitArray[11];
            DoCollisionAvoidance = bitArray[12];
            DoPositioningSystem = bitArray[13];
            DoInvite = bitArray[14];
            DoCompassCalibration = bitArray[15];
        }
    }
}
