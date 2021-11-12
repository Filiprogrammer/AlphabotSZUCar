using System;
using System.Collections;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class ToggleRequest : IAlphabotRequest
    {
        #region properties
        public bool DoCompassCalibration { get; set; }

        public bool DoInvite { get; set; }

        public bool DoPositioningSystem { get; set; }

        public bool DoCollisionAvoidance { get; set; }

        public bool DoNavigationMode { get; set; }

        public bool DoExploreMode { get; set; }

        public bool LogPositioning { get; set; }

        public bool LogObstacleDistance { get; set; }

        public bool LogPathfinderPath { get; set; }

        public bool LogCompassDirection { get; set; }

        public bool LogAnchorDistances { get; set; }

        public bool LogWheelSpeed { get; set; }

        public bool LogAccelerometer { get; set; }

        public bool LogGyroscope { get; set; }
        #endregion

        public ToggleRequest(ushort bitField)
        {
            byte[] bytes = BitConverter.GetBytes(bitField);
            BitArray bitArray = new BitArray(bytes);

            // Bits 0 and 1 are not used.
            DoExploreMode = bitArray[2];
            DoNavigationMode = bitArray[3];
            DoCollisionAvoidance = bitArray[4];
            DoPositioningSystem = bitArray[5];
            DoInvite = bitArray[6];
            DoCompassCalibration = bitArray[7];

            LogGyroscope = bitArray[8];
            LogAccelerometer = bitArray[9];
            LogWheelSpeed = bitArray[10];
            LogAnchorDistances = bitArray[11];
            LogCompassDirection = bitArray[12];
            LogPathfinderPath = bitArray[13];
            LogObstacleDistance = bitArray[14];
            LogPositioning = bitArray[15];
        }

        public BleInformation GetBleInformation()
        {
            return new BleInformation(BleUuids.TOGGLE, ConvertTogglesToBytes());
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[3];
            ret[0] = 0x05; // Packet ID 0x05

            byte[] toggleBytes = ConvertTogglesToBytes();
            ret[1] = toggleBytes[0];
            ret[2] = toggleBytes[1];

            return ret;
        }

        private byte[] ConvertTogglesToBytes() 
        {
            BitArray bitArray = new BitArray(16);

            // Bits 0 and 1 are not used.
            bitArray[2] = DoExploreMode;
            bitArray[3] = DoNavigationMode;
            bitArray[4] = DoCollisionAvoidance;
            bitArray[5] = DoPositioningSystem;
            bitArray[6] = DoInvite;
            bitArray[7] = DoCompassCalibration;

            bitArray[8] = LogGyroscope;
            bitArray[9] = LogAccelerometer;
            bitArray[10] = LogWheelSpeed;
            bitArray[11] = LogAnchorDistances;
            bitArray[12] = LogCompassDirection;
            bitArray[13] = LogPathfinderPath;
            bitArray[14] = LogObstacleDistance;
            bitArray[15] = LogPositioning;

            byte[] bytes = new byte[2];
            bitArray.CopyTo(bytes, 0);

            return bytes;
        }
    }
}
