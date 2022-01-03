using System;

namespace Alphabot.Net.Remote.Contracts
{
    public interface ISessionHandler
    {
        bool IsConnected { get; }
        void HandleSingleSession();
        void SendTextMessage(string message);
        void Close();

        event EventHandler SessionClosed;
    }
}
