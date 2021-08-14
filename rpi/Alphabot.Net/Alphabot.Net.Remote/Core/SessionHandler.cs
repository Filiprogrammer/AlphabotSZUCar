using System;
using System.Net.Sockets;
using Alphabot.Net.Remote.Contracts;
using Alphabot.Net.Shared;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Remote.Core
{
    /// <summary>
    /// This Class handles a Session. Every part of sending/receiving happens in this class.
    /// </summary>
    public class SessionHandler: ISessionHandler
    {
        #region fields

        private readonly Socket _clientSocket;
        private readonly IServiceLogger _serviceLogger = ServiceLogger.GetInstance().Current;
   
        private readonly ISocketWriter _socketWriter;
        private readonly ISocketReader _socketReader;

        private IProtocolParser _parser; 
        
        private readonly object _sync = new object();
        
        #endregion
        #region properties

        public bool IsConnected
        {
            get;
            private set;
        }
        #endregion
        #region ctor

        public SessionHandler(Socket clientSocket)
        {
            _clientSocket = clientSocket;
            IsConnected = true;
            _socketWriter = new SocketWriter(_clientSocket);
             _socketReader = new SocketReader(_clientSocket);
        }
        #endregion

        #region methods
        

        /// <summary>
        /// Every (single) client connection is handled by a specific SessionHandler instance. 
        /// It means a new SessionHandler instance is created for a client and handles all requests .
        /// Destroyed after the (single) client finished its activity.
        /// </summary>
        public void HandleSingleSession()
        {
            IAlphabotRequest alphabotRequest = null;
            while (true)
            {
                try
                {
                    _serviceLogger.Log(LogLevel.Information, "SessionHandler:HandleSingleSession",
                        $"waiting for request: {_clientSocket.RemoteEndPoint}" );
                    
                    _parser = new ProtocolParser(_socketReader.ReceiveText());
                    
                    _serviceLogger.Log(LogLevel.Information, "SessionHandler:HandleSingleSession",
                        $"request received: {_parser.Request}");
                    
                    if (_parser.GetCommand() == "exit" || _parser.GetCommand() == "shutdown")
                    {
                        _serviceLogger.Log(LogLevel.Information, "SessionHandler:HandleSingleSession",
                            "shutdown request received: " + _clientSocket.RemoteEndPoint);
                        break;
                    }

                    alphabotRequest = new AlphabotRequest(_parser.Request);
                    alphabotRequest.PerformAction();
                    
                    //sw.WriteBufferString(request.GetResponse());
                    _serviceLogger.Log(LogLevel.Debug, "SessionHandler:HandleSingleSession",
                        $"request done : {_parser.GetCommand()}");
                }
                catch (SocketException sx)
                {
                    _serviceLogger.Log(LogLevel.Warning, "SessionHandler:HandleSingleSession",
                        $"Tcp connection lost or shutdown by admin : {sx.Message}" );
                    IsConnected = false;
                    break;
                }
                catch (ObjectDisposedException ox)
                {
                    _serviceLogger.Log(LogLevel.Warning, "SessionHandler:HandleSingleSession",
                        $"Tcp connection lost or shutdown by admin : {ox.Message}" );
                    IsConnected = false;
                    break;
                }
                catch (TcpConnectionLostException tx)
                {
                    _serviceLogger.Log(LogLevel.Debug, "SessionHandler:HandleSingleSession",
                        tx.Message);
                    IsConnected = false;
                    break;
                }
                catch (Exception ex)
                {
                    _serviceLogger.Log(LogLevel.Warning, "SessionHandler:HandleSingleSession",
                        $"Error handling request : {ex.Message}\n{ex.InnerException}" );
                    IsConnected = false;
                    break;
                }
               
            }
            
            Close();
        }
        /// <summary>
        /// This method sends a text message and encodes it to bytes.
        /// </summary
        /// <param name="message">Message to send</param>
        public void SendTextMessage(string message)
        {
            lock (_sync)
            {
                _socketWriter.SendText(message);
            }
        }
        /// <summary>
        /// Close the session
        /// </summary>
        public void Close()
        {
            if (!IsConnected)
            {
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
            }
            // remove from list Sessions (in clientHandler) -> fire event 
            OnSessionClosed();
        }

        #endregion
        #region events 
        
        public event EventHandler SessionClosed;
        protected virtual void OnSessionClosed()
        {
            if (SessionClosed != null)
            {
                SessionClosed(this, EventArgs.Empty);
            }
           
        }
        #endregion

        
    }
}
