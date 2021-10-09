using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Core.Tcp
{
    public class TcpResponseInterpreter
    {
        private byte[] _bytes;

        public TcpResponseInterpreter(byte[] bytes)
        {
            _bytes = bytes;
        }

        public IAlphabotResponse GetResponse()
        {
            throw new NotImplementedException();
        }
    }
}
