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
            float[] axes = GetXYZAxes();
            return new MagnetometerResponse(axes[0], axes[1], axes[2]);
        }

        private AccelerometerResponse GetAccelerometerResponse()
        {
            float[] axes = GetXYZAxes();
            return new AccelerometerResponse(axes[0], axes[1], axes[2]);
        }

        private GyroscopeResponse GetGyroscopeResponse()
        {
            float[] axes = GetXYZAxes();
            return new GyroscopeResponse(axes[0], axes[1], axes[2]);
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

        private float[] GetXYZAxes()
        {
            byte[] xAxisBytes = new byte[4];
            byte[] yAxisBytes = new byte[4];
            byte[] zAxisBytes = new byte[4];
            Array.Copy(DataBytes, 0, xAxisBytes, 0, 4);
            Array.Copy(DataBytes, 4, yAxisBytes, 0, 4);
            Array.Copy(DataBytes, 8, zAxisBytes, 0, 4);
            float xAxis = BitConverter.ToSingle(xAxisBytes, 0);
            float yAxis = BitConverter.ToSingle(yAxisBytes, 0);
            float zAxis = BitConverter.ToSingle(zAxisBytes, 0);

            return new float[] { xAxis, yAxis, zAxis };
        }
    }
}
