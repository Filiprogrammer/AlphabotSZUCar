using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Remote.Contracts;

namespace Alphabot.Net.Remote.Core
{
    /// <summary>
    /// Provides a class for accepting and managing incomming connections using tcp
    /// </summary>
    public class ClientHandler:IClientHandler
    {
             #region fields
        private readonly Socket _serviceSocket;
        private readonly IServiceLogger _serviceLogger;
        private readonly List<ISessionHandler> _sessions;
        private object _sync = new object();
       
      //  private AlphabotDriveController _alphabotDrive = AlphabotDriveController.GetInstance();
        #endregion
        #region properties
        public IReadOnlyList<ISessionHandler> Sessions { get => _sessions; }
        #endregion

        #region constructors

        public ClientHandler(Socket serviceSocket)
        {
            _serviceSocket = serviceSocket;
            _serviceLogger = ServiceLogger.GetInstance().Current;
            _sessions = new List<ISessionHandler>();
 }
        #endregion
        #region methods
        /// <summary>
        /// This Method accepts incoming communincation requests and creates a client specific Socket and Session.
        /// </summary>
        public void AcceptClients()
        {
            Socket clientSocket = null;
            _serviceLogger.Log(LogLevel.Information, $"ServiceIP / Port: {_serviceSocket.LocalEndPoint}");
            while (true)
            {


                    _serviceLogger.Log(LogLevel.Information, "ClientHandler:AcceptClients", "waiting for incoming connection...");
                
                try
                {
                    clientSocket = _serviceSocket.Accept();
                  
                }
                catch (SocketException sx)
                {
                    _serviceLogger.Log(LogLevel.Information, "ClientHandler.AcceptClients", $"SocketException: {sx.Message}");
                    break;
                }
                catch (ObjectDisposedException ox)
                {
                    _serviceLogger.Log(LogLevel.Information, "ClientHandler.AcceptClients", $"ObjectDisposed: {ox.Message}");
                    break;
                }
                _serviceLogger.Log(LogLevel.Information, "ClientHandler:AcceptClients", $"client connected fom ip/port: {clientSocket.RemoteEndPoint}" ) ;


                var singleSession = new SessionHandler(clientSocket);
                singleSession.SessionClosed += SessionHandler_SessionClosed;
                RegisterSession(singleSession);
                
                
               // Action action = new Action(singleSession.HandleSingleSession);
               // var task = new Task(action);
               ThreadStart ts = new ThreadStart(singleSession.HandleSingleSession);
               Thread t = new Thread(ts);
               t.IsBackground = true;
               
                t.Start();
            }
        }

        public void Stop()
        {
            while (Sessions.Count > 0)
            {
                Sessions[0].Close();
                _serviceLogger.Log(LogLevel.Debug, "ClientHandler.Stop", "SingleSession closed.");
            }
            _serviceLogger.Log(LogLevel.Debug, "ClientHandler.Stop", "All sessions closed.");
            


        }
        #endregion
        
        /// <summary>
        /// Register a new session
        /// </summary>
        /// <param name="sessionHandler">"ISessionHandler to register"</param>
        private void RegisterSession(ISessionHandler sessionHandler)
        {
            lock (_sessions)
            {
                _sessions.Add(sessionHandler);
            }
        }
        /// <summary>
        /// Unregister session by reference
        /// </summary>
        /// <param name="sessionHandler">"ISessionHandler to unregister"</param>
        private void UnregisterSession(ISessionHandler sessionHandler)
        {
            lock (_sessions)
            {
               
                _sessions.Remove(sessionHandler);
            }
        }
        #region eventhandler
  
        private void SessionHandler_SessionClosed(object sender, EventArgs e)
        {
            UnregisterSession((ISessionHandler)sender);
        }
        #endregion
    }
}
