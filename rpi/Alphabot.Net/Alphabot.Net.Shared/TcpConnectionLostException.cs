using System;

namespace Alphabot.Net.Shared
{
    public class TcpConnectionLostException: Exception
    {
        public TcpConnectionLostException(string info): base(info)
        {
           
        }
    }
}
