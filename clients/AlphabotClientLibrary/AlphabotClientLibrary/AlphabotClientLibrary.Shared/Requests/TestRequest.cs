using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Requests
{
    public class TestRequest : IAlphabotRequest
    {
        public byte[] GetBytes()
        {
            byte[] test = new byte[2];
            test[0] = 99;
            test[1] = 99;

            return test;
        }

        public override string ToString()
        {
            return "TestRequest.cs";
        }
    }
}
