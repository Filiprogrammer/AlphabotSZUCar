using System;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class PingResponse : IAlphabotResponse
    {
        public long Time { get; private set; }

        public int Latency { get; private set; }

        public PingResponse(long time)
        {
            Time = time;
            Latency = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Time);
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.Ping;
        }
    }
}
