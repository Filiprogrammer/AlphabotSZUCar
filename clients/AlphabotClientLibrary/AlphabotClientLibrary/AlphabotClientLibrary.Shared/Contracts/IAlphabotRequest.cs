using System;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Contracts
{
    public interface IAlphabotRequest
    {
        byte[] GetBytes();

        BleInformation GetBleInformation();
    }
}
