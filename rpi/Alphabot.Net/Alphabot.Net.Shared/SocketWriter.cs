using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Shared
{
    /// <summary>
    /// This class sends messages and encodes them to bytes.
    /// </summary>
    public class SocketWriter:ISocketWriter
    {
        private readonly Socket _clientSocket;
        private readonly IServiceLogger _serviceLogger = ServiceLogger.GetInstance().Current;
        
        private NetworkStream _networkStream;
        public SocketWriter(Socket clientSocket)
        {
            _clientSocket = clientSocket;
        }
        public bool IsConnected => _clientSocket.Connected;
        public NetworkStream GetStream()
        {
            if (_networkStream == null) _networkStream = new NetworkStream(_clientSocket);

            return _networkStream;
        }



        /// <summary>
        /// This method sends a text message and encodes it to bytes.
        /// </summary
        /// <param name="message"></param>
        public void SendText(string message)
        {

            byte[] messageBuffer = Encoding.ASCII.GetBytes(message);
            try
            {
                _clientSocket.Send(messageBuffer);
            }
            catch (SocketException sx)
            {
                _serviceLogger.Log(LogLevel.Information, "Shared.SocketWriter.SendText", "SocketException: " + sx.Message);
            }
            catch (ObjectDisposedException ox)
            {
                _serviceLogger.Log(LogLevel.Information, "Shared.SocketWriter.SendText", "ObjectDisposedException: " + ox.Message);
            }
        }
        /// <summary>
        /// Sends a text message to a connected socket - including Environment.NewLine as seperator
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message)
        {
            
            var sw = new StreamWriter(GetStream());
            sw.WriteLine(message);
        }
        /// <summary>
        /// closes the remote host connection and releases all managed and unmanaged resources associated with the Socket.
        /// Upon closing, the Connected property is set to false.
        /// </summary>
        public void Close()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
    }
}
