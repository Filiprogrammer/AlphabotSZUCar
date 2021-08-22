using System;
using System.Net;
using System.Net.Sockets;

namespace AlphabotTcpClient
{
    public class AlphabotClient
    {
        #region fields
        private readonly IPEndPoint _serviceEndPoint;

        private Socket _clientSocket;
        private SocketWriter _socketWriter;
        private SocketReader _socketReader;
        #endregion

        #region constructor
        public AlphabotClient(string address, ushort port)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(address);

            if (addresses.Length == 0)
                throw new ArgumentException("Host name could not be resolved");

            _serviceEndPoint = new IPEndPoint(addresses[addresses.Length - 1], port);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketWriter = new SocketWriter(_clientSocket);
            _socketReader = new SocketReader(_clientSocket);
        }
        #endregion

        #region networking
        public void Connect()
        {
            _clientSocket.Connect(_serviceEndPoint);
        }

        public void Shutdown()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
        #endregion
        
        public void SendString(string cmd)
        {
            _socketWriter.WriteString(cmd); 
        }

        public string ReceiveString()
        {
            if (_socketReader == null)
                throw new InvalidOperationException("Client has not been started yet");

            return _socketReader.ReadString();
        }
    }
}
