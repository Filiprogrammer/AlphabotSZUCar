using System;
using System.Net.Sockets;
using Alphabot.Net.Remote.Contracts;
using Alphabot.Net.Shared;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Shared.Responses;
using AlphabotClientLibrary.Shared.Contracts;

namespace Alphabot.Net.Remote.Core
{
    /// <summary>
    /// This Class handles a Session. Every part of sending/receiving happens in this class.
    /// </summary>
    public class SessionHandler : ISessionHandler
    {
        #region fields

        private readonly Socket _clientSocket;
        private readonly IServiceLogger _serviceLogger = ServiceLogger.GetInstance().Current;

        private readonly ISocketWriter _socketWriter;
        private readonly ISocketReader _socketReader;
        private DateTime _lastSentPositioningTime;

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
            // send current toggle settings at start
            SendResponseEvent(ToggleSettings.GetInstance.GetToggleResponse());

            //turn positioningsystem on by default
            SystemHandler.GetInstance.PositioningSystem = new Car.Devices.PositioningSystem(0x0CBE, 0x4CA6, 0x1782, SendResponseEvent);

            IAlphabotRequest alphabotRequest = null;
            while (true)
            {
                try
                {
                    _serviceLogger.Log(LogLevel.Information, "SessionHandler:HandleSingleSession",
                        $"waiting for request: {_clientSocket.RemoteEndPoint}");

                    alphabotRequest = new BitProtocolParser(this._socketReader.ReceiveBytes(), SendResponseEvent).GetRequest();

                    if (alphabotRequest != null)
                    {
                        _serviceLogger.Log(LogLevel.Information, "SessionHandler:HandleSingleSession",
                        $"request received: {alphabotRequest.ToString()}");
                        new ActionExecutor(alphabotRequest, SendResponseEvent).Perform();
                    }
                    else
                    {
                        _serviceLogger.Log(LogLevel.Warning, "SessionHandler:HandleSingleSession",
                       $"request was null, this could be because of an error");
                    }
                }
                catch (SocketException sx)
                {
                    _serviceLogger.Log(LogLevel.Warning, "SessionHandler:HandleSingleSession",
                        $"Tcp connection lost or shutdown by admin : {sx.Message}");
                    IsConnected = false;
                    break;
                }
                catch (ObjectDisposedException ox)
                {
                    _serviceLogger.Log(LogLevel.Warning, "SessionHandler:HandleSingleSession",
                        $"Tcp connection lost or shutdown by admin : {ox.Message}");
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
                        $"Error handling request : {ex.Message}\n{ex.InnerException}");
                    IsConnected = false;
                    break;
                }
            }
            Close();
        }

        private void SendResponseEvent(IAlphabotResponse response)
        {
            if (response is PositioningResponse)
            {
                if (!ToggleSettings.GetInstance.LogPositioning)
                {
                    // if log positioning is deactivated, dont send it
                    return;
                }

                if ((DateTime.Now - _lastSentPositioningTime).TotalMilliseconds < 500)
                {
                    return;
                }
                _lastSentPositioningTime = DateTime.Now;
            }
            else if (response is ErrorResponse)
            {
                _serviceLogger.Log(LogLevel.Warning, "SessionHandler:SendResponseEvent",
                        $"The following error happened, details were sent to client: {(response as ErrorResponse).Error.ToString()}");
            }

            _serviceLogger.Log(LogLevel.Information, "SessionHandler:SendResponseEvent",
                        $"Sent response : {response.ToString()}");
            this._socketWriter.SendBytes(response.GetBytes());
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
