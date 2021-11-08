using System;

namespace AlphabotClientLibrary.Shared.Contracts
{
    public interface IResponseInterpreter
    {
        IAlphabotResponse GetResponse();
    }
}
