using System.Net.Sockets;

namespace Alphabot.Net.Shared.Contracts
{
    public interface ISocketReader
    {
        NetworkStream GetStream();
        string ReceiveText();
        byte[] ReceiveBytes();
        string ReadLine();
        void Close();
    }
}
