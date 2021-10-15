using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class PingResponse : IAlphabotResponse
    {
        public long Time { get; private set; }
        public PingResponse(long time)
        {
            Time = time;
        }
    }
}
