using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class RemoveAllObstaclesRequest : IAlphabotRequest
    {
        public RemoveAllObstaclesRequest() { }

        public BleInformation GetBleInformation()
        {
            //Not added in documentaion?
            throw new NotImplementedException();
        }

        public byte[] GetBytes()
        {
            byte[] packetId = { 0x0C };

            return packetId;
        }
    }
}
