using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class CalibrateCompassRequest : IAlphabotRequest
    {
        private CompassCalibrationType _compassCalibrationType;

        public enum CompassCalibrationType
        {
            StartAutomated = 0x01,
            StartManual = 0x02,
            EndManual = 0x03,
            SetCompassOffset = 0x04
        }

        public CalibrateCompassRequest(CompassCalibrationType compassCalibrationType)
        {
            _compassCalibrationType = compassCalibrationType;
        }

        public BleInformation GetBleInformation()
        {
            byte[] bytes = new byte[1];

            switch(_compassCalibrationType)
            {
                case CompassCalibrationType.StartAutomated:
                    bytes[0] = 0x02;
                    break;
                case CompassCalibrationType.StartManual:
                    bytes[0] = 0x03;
                    break;
                case CompassCalibrationType.EndManual:
                    bytes[0] = 0x00;
                    break;
                case CompassCalibrationType.SetCompassOffset:
                    bytes[0] = 0x04;
                    break;
            }

            return new BleInformation(BleUuids.CALIBRATE, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[2];
            ret[0] = 0x04; // Packet ID 0x04
            ret[1] = (byte)_compassCalibrationType;
            return ret;
        }
    }
}
