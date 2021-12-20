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
                    return GetAnchorDistancesResponse();
                case 0x09:
                    return GetWheelSpeedResponse();
                case 0x0A:
                    return GetGyroscopeResponse();
                case 0x0B:
                    return GetAccelerometerResponse();
                case 0x0C:
                    return GetMagnetometerResponse();
                case 0x0D:
                    return GetErrorResponse();
            }

            throw new Exception("Received packet id is not valid");
        }

        private IAlphabotResponse GetMagnetometerResponse()
        {
            short[] axes = GetXYZAxes();
            return new MagnetometerResponse(axes[0] / 10f, axes[1] / 10f, axes[2] / 10f);
        }

        private AccelerometerResponse GetAccelerometerResponse()
        {
            short[] axes = GetXYZAxes();
            return new AccelerometerResponse(axes[0] / 1000f, axes[1] / 1000f, axes[2] / 1000f);
        }

        private GyroscopeResponse GetGyroscopeResponse()
        {
            short[] axes = GetXYZAxes();
            return new GyroscopeResponse(axes[0] / 10f, axes[1] / 10f, axes[2] / 10f);
        }

        private WheelSpeedResponse GetWheelSpeedResponse()
        {
            sbyte speed = (sbyte)(DataBytes[0]);
            return new WheelSpeedResponse(speed);
        }

        private AnchorDistancesResponse GetAnchorDistancesResponse()
        {
            ushort distanceAnchor0 = (ushort)(DataBytes[0] | (DataBytes[1] << 8));
            ushort distanceAnchor1 = (ushort)(DataBytes[2] | (DataBytes[3] << 8));
            ushort distanceAnchor2 = (ushort)(DataBytes[4] | (DataBytes[5] << 8));

            return new AnchorDistancesResponse(distanceAnchor0, distanceAnchor1, distanceAnchor2);
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

        private short[] GetXYZAxes()
        {
            short xAxis = (short)(DataBytes[0] | (DataBytes[1] << 8));
            short yAxis = (short)(DataBytes[2] | (DataBytes[3] << 8));
            short zAxis = (short)(DataBytes[4] | (DataBytes[5] << 8));

            return new short[] { xAxis, yAxis, zAxis };
        }
    }
}
