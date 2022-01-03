using System.Net.Sockets;

namespace Alphabot.Net.Shared.Contracts
{
    public interface ISocketWriter
    {
        NetworkStream GetStream();
        void SendText(string message);
        void SendBytes(byte[] bytes);
        void WriteLine(string message);
        void Close();
    }
}
