using Alphabot.Net.Remote.Contracts;
using Alphabot.Net.Shared;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphabot.Net.Remote.Core
{
    public class BitProtocolParser
    {
        byte[] _bytes;

        public BitProtocolParser(byte[] bytes)
        {
            _bytes = bytes;
        }

        public IAlphabotRequest GetRequest()
        {
            switch (_bytes[0])
            {
                case 0x01:
                    return GetSpeedAndSteerRequest();
                case 0x03:
                    return new CalibrateSteeringRequest();
                case 0x05:
                    return GetToggleRequest();
                case 0x06:
                    return GetPingRequest();
                case 0x07:
                    return GetConfigurePositioningAnchorRequest();
            }
            return null;
        }

        private PingRequest GetPingRequest()
        {
            byte[] dataBytes = new byte[8];
            Array.Copy(_bytes, 1, dataBytes, 0, 8);

            return new PingRequest(dataBytes);
        }

        private ConfigurePositioningAnchorRequest GetConfigurePositioningAnchorRequest()
        {
            byte anchorId = _bytes[1];
            byte[] positionBytes = new byte[4];
            Array.Copy(_bytes, 2, positionBytes, 0, 4);

            return new ConfigurePositioningAnchorRequest(anchorId, new Position(positionBytes));
        }

        private SpeedSteerRequest GetSpeedAndSteerRequest()
        {
            sbyte speed = (sbyte)_bytes[1];
            sbyte steer = (sbyte)_bytes[2];

            return new SpeedSteerRequest(steer, speed);
        }

        private ToggleRequest GetToggleRequest()
        {
            byte[] toggleBytes = new byte[2];

            Array.Copy(_bytes, 1, toggleBytes, 0, 2);

            return new ToggleRequest(toggleBytes);
        }
    }
}
