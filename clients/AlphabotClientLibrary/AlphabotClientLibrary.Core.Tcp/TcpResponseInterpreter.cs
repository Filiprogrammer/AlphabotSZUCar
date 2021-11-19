using System;
using AlphabotClientLibrary.Shared;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;
using AlphabotClientLibrary.Shared.Responses;

namespace AlphabotClientLibrary.Core.Tcp
{
    public class TcpResponseInterpreter : ResponseInterpreter
    {
        private byte _headerByte;

        public TcpResponseInterpreter(byte[] bytes)
        {
            _headerByte = bytes[0];
            DataBytes = new byte[bytes.Length - 1];
            Array.Copy(bytes, 1, DataBytes, 0, bytes.Length - 1);
        }

        public override IAlphabotResponse GetResponse()
        {
            switch (_headerByte)
            {
                case 0x01:
                    return GetDistanceSensorResponse();
                case 0x02:
                    return GetPositioningResponse();
                case 0x03:
                    return GetPathFindingResponse();
                case 0x04:
                    return GetCompassResponse();
                case 0x05:
                    return GetPingResponse();
                case 0x06:
                    return GetNewObstacleRegisteredResponse();
                case 0x07:
                    return GetToggleResponse();
                case 0x08:
                    return GetErrorResponse();
            }

            throw new Exception("Received packet id is not valid");
        }

        private DistanceSensorResponse GetDistanceSensorResponse()
        {
            short degree = (short)(DataBytes[0] | (DataBytes[1] << 8));
            ushort distance = (ushort)(DataBytes[2] | (DataBytes[3] << 8));

            return new DistanceSensorResponse(degree, distance);
        }

        private PositioningResponse GetPositioningResponse()
        {
            byte[] positioningBytes = new byte[4];
            Array.Copy(DataBytes, positioningBytes, 4);

            return new PositioningResponse(new Position(positioningBytes));
        }

        private CompassResponse GetCompassResponse()
        {
            short degree = (short)(DataBytes[0] | (DataBytes[1] << 8));
            return new CompassResponse(degree);
        }
    }
}
