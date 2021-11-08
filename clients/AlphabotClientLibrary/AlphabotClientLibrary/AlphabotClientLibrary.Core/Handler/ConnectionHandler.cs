using System;
using AlphabotClientLibrary.Core.Handler;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Core
{
    public abstract class ConnectionHandler
    {
        public ResponseHandler ResponseHandler { get; private set; }

        protected IConnectionData _connectionData;

        public ConnectionHandler(ResponseHandler responseHandler)
        {
            ResponseHandler = responseHandler;
        }

        public abstract bool Connect(IConnectionData connectionData);

        public abstract void Disconnect();

        public abstract bool SendAction(IAlphabotRequest request);
    }
}
