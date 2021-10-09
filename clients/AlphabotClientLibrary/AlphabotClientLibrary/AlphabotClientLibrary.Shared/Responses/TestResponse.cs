using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
