using System;
using System.Net.Sockets;
using System.Text;

namespace AlphabotTcpClient
{
    /// <summary>
    /// Send messages to a socket.
    /// </summary>
    class SocketWriter
    {
        private Socket _clientSocket;

        public SocketWriter(Socket clientSocket)
        {
            _clientSocket = clientSocket ?? throw new ArgumentNullException("clientSocket must not be null");
        }

        /// <summary>
        /// Send the given bytes to the socket.
        /// </summary>
        /// <param name="msg">bytes to send to the client</param>
        public void WriteBytes(byte[] msg)
        {
            _clientSocket.Send(msg);
        }

        /// <summary>
        /// Send the given text string to the socket.
        /// </summary>
        /// <param name="msg">text string to send to the client</param>
        public void WriteString(string msg)
        {
            if (msg == null)
                throw new ArgumentNullException("msg must not be null");

            byte[] message = Encoding.ASCII.GetBytes(msg);
            _clientSocket.Send(message);
        }
    }
}
