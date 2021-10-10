using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Contracts
{
    public interface IAlphabotRequest
    {
        public byte[] GetBytes();

        public BleInformation GetBleInformation();
    }
}
