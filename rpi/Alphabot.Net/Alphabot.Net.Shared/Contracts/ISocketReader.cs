using System.Net.Sockets;

namespace Alphabot.Net.Shared.Contracts
{
    public interface ISocketReader
    {
        NetworkStream GetStream();
        string ReceiveText();
        string ReadLine();
        void Close();
    }
}
