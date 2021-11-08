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

        public ToggleRequest() { }

        public ToggleRequest(ushort bitField)
        {
            byte[] bytes = BitConverter.GetBytes(bitField);

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

        public BleInformation GetBleInformation()
        {
            Guid uuid = new Guid("fce001d4-864a-48f4-9c95-de928f1da07b");
            byte[] bytes = ConvertTogglesToBytes();

            return new BleInformation(uuid, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[3];

            ret[0] = 0x05; //Packet ID 0x05

            byte[] toggleBytes = ConvertTogglesToBytes();

            ret[1] = toggleBytes[0];
            ret[2] = toggleBytes[1];

            return ret;
        }

        private byte[] ConvertTogglesToBytes() 
        {
            BitArray bitArray = new BitArray(16);

            bitArray[0] = LogGyroscope;
            bitArray[1] = LogAccelerometer;
            bitArray[2] = LogWheelSpeed;
            bitArray[3] = LogAnchorDistances;
            bitArray[4] = LogCompassDirection;
            bitArray[5] = LogPathfinderPath;
            bitArray[6] = LogObstacleDistance;
            bitArray[7] = LogPositioning;

            //bit 8 and 9 are not used

            bitArray[10] = DoExploreMode;
            bitArray[11] = DoNavigationMode;
            bitArray[12] = DoCollisionAvoidance;
            bitArray[13] = DoPositioningSystem;
            bitArray[14] = DoInvite;
            bitArray[15] = DoCompassCalibration;

            byte[] bytes = new byte[2];

            bitArray.CopyTo(bytes, 0);

            //Swap the two bytes, so the order is correct
            byte temp = bytes[0];
            bytes[0] = bytes[1];
            bytes[1] = temp;

            return bytes;
        }
    }
}
