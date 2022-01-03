using System;
using Alphabot.Net.Shared;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Requests;
using Alphabot.Net.Shared.Responses;

namespace Alphabot.Net.Remote.Core
{
    public class BitProtocolParser
    {
        byte[] _bytes;
        Delegates.ResponseSender _responseSender;

        public BitProtocolParser(byte[] bytes, Delegates.ResponseSender responseSender)
        {
            _bytes = bytes;
            _responseSender = responseSender;
        }

        public IAlphabotRequest GetRequest()
        {
            byte protocolVersion = (byte)(_bytes[0] >> 5);
            byte packetId = (byte)(_bytes[0] & 31);

            // current protocol version is 0
            if (protocolVersion != 0)
            {
                _responseSender(new ErrorResponse(ErrorResponse.ErrorType.UnknownProtocol, _bytes));
                return null;
            }

            switch (packetId)
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
            _responseSender(new ErrorResponse(ErrorResponse.ErrorType.UnknownPacketId, _bytes));
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

            if (anchorId > 2)
            {
                _responseSender(new ErrorResponse(ErrorResponse.ErrorType.WrongPayload, _bytes));
                return null;
            }

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
