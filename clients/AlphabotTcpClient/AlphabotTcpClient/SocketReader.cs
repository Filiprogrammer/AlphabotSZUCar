using System;
using System.Net.Sockets;
using System.Text;

namespace AlphabotTcpClient
{
    /// <summary>
    /// Receive messages from a socket.
    /// </summary>
    class SocketReader
    {
        private Socket _clientSocket;
        private const int SOCKET_BUFFER_SIZE = 2048;

        public SocketReader(Socket clientSocket)
        {
            _clientSocket = clientSocket ?? throw new ArgumentNullException("clientSocket must not be null");
        }

        /// <summary>
        /// Wait and receive bytes from the socket.
        /// </summary>
        /// <param name="bufferSize">maximum number of bytes to receive</param>
        /// <returns>received bytes</returns>
        public byte[] ReadBytes(int bufferSize = SOCKET_BUFFER_SIZE)
        {
            byte[] buffer = new byte[bufferSize];
            int len = 0;

            try
            {
                len = _clientSocket.Receive(buffer);
            }
            catch (SocketException) { }

            byte[] data = new byte[len];
            Array.Copy(buffer, data, len);
            return data;
        }

        /// <summary>
        /// Wait and receive text string from the socket.
        /// </summary>
        /// <param name="bufferSize">maxmimum number of bytes to receive</param>
        /// <returns>received bytes</returns>
        public string ReadString(int bufferSize = SOCKET_BUFFER_SIZE)
        {
            byte[] msg = ReadBytes(bufferSize);
            return Encoding.ASCII.GetString(msg);
        }
    }
}
