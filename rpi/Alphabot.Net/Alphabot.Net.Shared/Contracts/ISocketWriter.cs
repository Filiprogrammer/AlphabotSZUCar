using System.Net.Sockets;

namespace Alphabot.Net.Shared.Contracts
{
    public interface ISocketWriter
    {
        NetworkStream GetStream();
        void SendText(string message);
        void WriteLine(string message);
        void Close();
    }
}
