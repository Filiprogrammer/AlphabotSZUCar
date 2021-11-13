using System;
using System.Collections.Generic;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Responses;

namespace AlphabotClientLibrary.Core.Ble
{
    public class BleSensorResponse
    {
        private byte[] _bytes;
        private List<IAlphabotResponse> _sensorPackets = new List<IAlphabotResponse>();

        public BleSensorResponse(byte[] bytes)
        {
            _bytes = bytes;
        }

        public MultipleSensorResponse GetMultipleSensorResponse()
        {
            int offset = 2;

            for (int i = 0; i < 8; ++i)
            {
                int sensorTypeNum = (_bytes[i / 4] >> ((i * 2) % 8)) & 0x03;
                MultipleSensorResponse.SensorType sensorType = (MultipleSensorResponse.SensorType)sensorTypeNum;

                if (sensorType == MultipleSensorResponse.SensorType.None)
                    break;

                switch (sensorType)
                {
                    case MultipleSensorResponse.SensorType.DistanceSensor:
                        AddDistanceSensorResponse(offset);
                        offset += 2;
                        break;
                    case MultipleSensorResponse.SensorType.Positioning:
                        AddPositioningResponse(offset);
                        offset += 3;
                        break;
                    case MultipleSensorResponse.SensorType.Compass:
                        AddCompassResponse(offset);
                        offset += 2;
                        break;
                }
            }

            return new MultipleSensorResponse(_sensorPackets);
        }

        private void AddCompassResponse(int index)
        {
            short degree = (short)(_bytes[0] | ((_bytes[1] << 8) & 0x0F));
            _sensorPackets.Add(new CompassResponse(degree));
        }

        private void AddPositioningResponse(int index)
        {
            short posX = (short) (_bytes[index] | ((_bytes[index + 1] << 8) & 0x0F));
            short posY = (short) (((_bytes[index + 1] & 0xF0) >> 4) | (_bytes[index + 2] << 4));

            _sensorPackets.Add(new PositioningResponse(new Shared.Models.Position(posX, posY)));
        }

        private void AddDistanceSensorResponse(int index)
        {
            short degree = (short)(_bytes[index] * 2);
            ushort distance = (ushort)(_bytes[index + 1] * 2);

            _sensorPackets.Add(new DistanceSensorResponse(degree, distance));
        }
    }
}
