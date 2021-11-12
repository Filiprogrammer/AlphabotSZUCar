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
            if (_uuid == BleUuids.TOGGLE)
                return GetToggleResponse();
            else if (_uuid == BleUuids.SENSOR)
                return new BleSensorResponse(DataBytes).GetMultipleSensorResponse();
            else if (_uuid == BleUuids.CALIBRATE)
                //there is no calibrate response class because it is not in tcp?
                throw new NotImplementedException();
            else if (_uuid == BleUuids.ADD_OBSTACLE)
                return GetNewObstacleRegisteredResponse();
            else if (_uuid == BleUuids.PATH_FINDING_PATH)
                return GetPathFindingResponse();
            else if (_uuid == BleUuids.ERROR)
                return GetErrorResponse();

            return null;
        }
    }
}
