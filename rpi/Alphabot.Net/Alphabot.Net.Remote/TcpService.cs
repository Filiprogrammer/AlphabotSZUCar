using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Remote.Contracts;
using Alphabot.Net.Remote.Core;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Remote
{
    /// <summary>
    /// Provides a class for a tcp service that will exist as part of a service application.
    /// Based on standard Berkley sockets.
    /// </summary>
    public class TcpService: IService
    {
        #region fields

        private readonly Prefs _prefs = Prefs.GetInstance();
        private readonly IServiceLogger _serviceLogger = ServiceLogger.GetInstance().Current;
        private IPEndPoint _serviceEndPoint;
        private readonly Socket _serviceSocket;
        
        private readonly ClientHandler _clientHandler;

        #endregion

        #region properties

        public IPEndPoint ServiceEndPoint
        {
            get => _serviceEndPoint;
            set => _serviceEndPoint = value;
        }

        #endregion
        

        #region constructors

        /// <summary>
        /// Initializes a new instance of the TcpService class for a communication port and logging destination
        /// </summary>
        /// <param name="port">port number for the service</param>
        /// <param name="serviceLogger">concrete logging instance</param>
        public TcpService(int port)
        {
           
            // TODO: auf allen ip's 
            ServiceEndPoint = new IPEndPoint(IPAddress.Any, port);
            _serviceSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientHandler = new ClientHandler(_serviceSocket);
        }

        #endregion

        #region methods

        /// <summary>
        /// Puts the tcp service in the Accept - State
        /// multiple clients - stopable
        /// </summary>
        /// <exception cref="ObjectDisposedException">ServiceSocket doesn't exist.</exception>
        public void Start()
        {
            _serviceLogger.Log(LogLevel.Information, "TcpSocketService:Start", "Alphabot service starting ...");
            try
            {
                _serviceSocket.Bind(ServiceEndPoint);
                _serviceSocket.Listen(254);
            }
            catch (ObjectDisposedException oex)
            {
                _serviceLogger.Log(LogLevel.Error, "TcpSocketService:Start",
                    "failed to start service - message: " + oex.Message);
            }

          //  Action action = new Action(_clientHandler.AcceptClients);
          //  Task t = new Task(action);

          ThreadStart ts = new ThreadStart(_clientHandler.AcceptClients);
          Thread t = new Thread(ts) {IsBackground = true};

          t.Start();
        }

        /// <summary>
        /// Stops the tcp service
        /// </summary>
        public void Stop()
        {
            _serviceLogger.Log(LogLevel.Information, "TcpSocketService:Stop", "service shutting down ...");

            _serviceSocket.Shutdown(SocketShutdown.Both);
            _serviceSocket.Close();
            
            _clientHandler.Stop();
            
            _serviceLogger.Log(LogLevel.Information, "TcpSocketService:Stop", "shutting down successfully ...");
        }

        /// <summary>
        /// Restarts the tcp service
        /// </summary>
        public void Restart()
        {
            Stop();
            Thread.Sleep(2000);
            Start();
        }

        #endregion
    }
}
