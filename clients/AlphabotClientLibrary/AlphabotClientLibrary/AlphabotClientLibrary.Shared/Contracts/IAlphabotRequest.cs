using System;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Contracts
{
    public interface IAlphabotRequest
    {
        public byte[] GetBytes();

        public BleInformation GetBleInformation();
    }
}
