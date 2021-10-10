using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class SpeedSteerRequest : IAlphabotRequest
    {
        sbyte _steer;
        sbyte _speed;
        public SpeedSteerRequest(sbyte steer, sbyte speed)
        {
            _steer = steer;
            _speed = speed;
        }

        public BleInformation GetBleInformation()
        {
            Guid uuid = new Guid("a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2");
            byte[] bytes = new byte[2];

            bytes[0] = (byte) _speed;
            bytes[1] = (byte) _steer;

            return new BleInformation(uuid, bytes);
        }

        public byte[] GetBytes()
        {
            byte[] ret = new byte[3];

            ret[0] = 0x01; //Packet ID 0x01

            ret[1] = (byte) _speed;
            ret[2] = (byte) _steer;

            return ret;
        }
    }
}
