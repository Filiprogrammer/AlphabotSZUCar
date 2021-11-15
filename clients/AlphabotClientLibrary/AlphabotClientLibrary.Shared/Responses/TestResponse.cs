using System;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class TestResponse : IAlphabotResponse
    {
        public override string ToString()
        {
            return "TestResponse.cs";
        }
    }
}
