using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Shared
{
    /// <summary>
    /// This class receives incoming messages and encodes them to ASCII.
    /// </summary>
    public class SocketReader: ISocketReader
    {
        private readonly Socket _clientSocket;
        private readonly IServiceLogger _serviceLogger = ServiceLogger.GetInstance().Current;
        Prefs _prefs = Prefs.GetInstance();
        private NetworkStream _networkStream;

        public NetworkStream GetStream()
        {
            if (_networkStream == null) _networkStream = new NetworkStream(_clientSocket);

            return _networkStream;
        }
        public bool IsConnected => _clientSocket.Connected;
        public SocketReader(Socket clientSocket)
        {
            _clientSocket = clientSocket;
        }
        /// <summary>
        /// This method receives a incoming message and encodes it to ASCII.
        /// </summary>
        /// <returns>encoded message</returns>
        public string ReceiveText()
        {
            byte[] msg = new byte[_prefs.ServiceSettings.SocketBufferSize];
            try
            {
               var size= _clientSocket.Receive(msg);
       
               if (size == 0) // connection lost
               {
                   throw new TcpConnectionLostException("0 Bytes received: Tcp connection lost");
               }
               _serviceLogger.Log(LogLevel.Debug, "Shared.SocketReader.ReceiveText", $"bytes received: {size}" );
            }
            catch (SocketException sx)
            {
                _serviceLogger.Log(LogLevel.Information, "Shared.SocketWriter.ReceiveText", "SocketException: " + sx.Message);
            }
            catch (ObjectDisposedException ox)
            {
                _serviceLogger.Log(LogLevel.Information, "Shared.SocketWriter.ReceiveText", "ObjectDisposedException: " + ox.Message);

            }
            string s = Encoding.ASCII.GetString(msg);
            s = s.Substring(0, s.IndexOf('\0'));
            return s;
        }
        /// <summary>
        /// This method receives a incoming message and encodes it to ASCII - interpreting Environment.NewLine als seperator
        /// </summary>
        /// <returns>encoded message</returns>
        public string ReadLine()
        {
            var sr = new StreamReader(GetStream());
            return sr.ReadLine();
        }
        public byte[] ReceiveBytes()
        {
            byte[] msg = new byte[this._prefs.ServiceSettings.SocketBufferSize];
            try
            {
                int size = this._clientSocket.Receive(msg);
                if (size == 0)
                {
                    throw new TcpConnectionLostException("0 Bytes received: Tcp connection lost");
                }
                this._serviceLogger.Log(LogLevel.Debug, "Shared.SocketReader.ReceiveText", string.Format("bytes received: {0}", size));
            }
            catch (SocketException sx)
            {
                this._serviceLogger.Log(LogLevel.Information, "Shared.SocketWriter.ReceiveText", "SocketException: " + sx.Message);
            }
            catch (ObjectDisposedException ox)
            {
                this._serviceLogger.Log(LogLevel.Information, "Shared.SocketWriter.ReceiveText", "ObjectDisposedException: " + ox.Message);
            }
            return msg;
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
