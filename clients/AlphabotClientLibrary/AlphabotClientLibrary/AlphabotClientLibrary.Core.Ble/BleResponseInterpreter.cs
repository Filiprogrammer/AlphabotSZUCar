using System;
using AlphabotClientLibrary.Shared;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Core.Ble
{
    public class BleResponseInterpreter : ResponseInterpreter
    {
        Guid _uuid;

        public BleResponseInterpreter(BleInformation bleInformation)
        {
            DataBytes = bleInformation.Bytes;
            _uuid = bleInformation.Uuid;
        }

        public override IAlphabotResponse GetResponse()
        {
            switch (_uuid.ToString())
            {
                case "a04295f7-eaa8-4536-b3a4-4e7ae4d72dc2":
                    return GetPingResponse();
                case "fce001d4-864a-48f4-9c95-de928f1da07b":
                    return GetToggleResponse();
                case "4c999381-35e2-4af4-8443-ee8b9fe56ba0":
                    return new BleSensorResponse(DataBytes).GetMultipleSensorResponse();
                case "d39e8d54-8019-46c8-a977-db13871bac59":
                    //there is no calibrate response class because it is not in tcp?
                    throw new NotImplementedException();
                case "60db37c7-afeb-4d40-bb17-a19a07d6fc95":
                    return GetNewObstacleRegisteredResponse();
                case "8dad4c9a-1a1c-4a42-a522-ded592f4ed99":
                    return GetPathFindingResponse();
                case "dc458f08-ea3e-4fe1-adb3-25c840be081a":
                    return GetErrorResponse();
            }
            return null;
        }
    }
}
