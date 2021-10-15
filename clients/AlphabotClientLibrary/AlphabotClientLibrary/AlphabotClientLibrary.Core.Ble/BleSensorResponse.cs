using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Responses;

namespace AlphabotClientLibrary.Core.Ble
{
    public class BleSensorResponse
    {
        private byte[] _bytes;
        private List<IAlphabotResponse> _sensorPackets;

        public BleSensorResponse(byte[] bytes)
        {
            _bytes = bytes;
        }

        public MultipleSensorResponse GetMultipleSensorResponse()
        {
            _sensorPackets = new List<IAlphabotResponse>();
            int bytePointer = 2;

            BitArray bitArray1 = new BitArray(_bytes[0]);
            BitArray bitArray2 = new BitArray(_bytes[1]);

            bool[] arr1 = new bool[8];
            bool[] arr2 = new bool[8];

            bitArray1.CopyTo(arr1, 0);
            bitArray2.CopyTo(arr2, 0);

            arr1.Reverse();
            arr2.Reverse();

            bool[] sensortypeArray = new bool[16];

            sensortypeArray = arr1.Concat(arr2).ToArray();

            for (int bitPointer = 0; bitPointer < 16; bitPointer+=2)
            {
                if(sensortypeArray[bitPointer])
                {
                    if(sensortypeArray[bitPointer+1])
                    {
                        // 11 compass response
                        AddCompassResponse(bytePointer);
                        bytePointer += 2;
                    }
                    else
                    {
                        // 10 positioning response
                        AddPositioningResponse(bytePointer);
                        bytePointer += 3;
                    }
                }
                else
                {
                    if(sensortypeArray[bitPointer + 1])
                    {
                        //01 Distance sensor response
                        AddDistanceSensorResponse(bytePointer);
                        bytePointer += 2;
                    }
                    else
                    {
                        //00 end of sensors
                        break;
                    }
                }
            }

            return new MultipleSensorResponse(_sensorPackets);
        }

        private void AddCompassResponse(int index)
        {
            byte[] bytes = new byte[2];

            Array.Copy(_bytes, index, bytes, 0, 2);

            short degree = BitConverter.ToInt16(bytes);

            _sensorPackets.Add(new CompassResponse(degree));
        }

        private void AddPositioningResponse(int index)
        {
            byte[] bytes = new byte[3];

            Array.Copy(_bytes, index, bytes, 0, 3);

            short posX = (short) (bytes[0] | ((bytes[1] << 8) & 0x0F));

            short posY = (short) (((bytes[1] & 0xF0) >> 4) | (bytes[2] << 4));

            _sensorPackets.Add(new PositioningResponse(new Shared.Models.Position(posX, posY)));
        }

        private void AddDistanceSensorResponse(int index)
        {
            byte[] bytes = new byte[2];

            Array.Copy(_bytes, index, bytes, 0, 2);

            short degree = (short)(bytes[0] * 2);
            ushort distance = (ushort)(bytes[1] * 2);

            _sensorPackets.Add(new DistanceSensorResponse(degree, distance));
        }

    }
}
