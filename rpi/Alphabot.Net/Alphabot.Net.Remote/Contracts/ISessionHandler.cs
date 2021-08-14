using System;

namespace Alphabot.Net.Remote.Contracts
{
    public interface ISessionHandler
    {
       
      //  IUser User { get;  }
        bool IsConnected { get; }
        void HandleSingleSession();
        void SendTextMessage(string message);
        void Close();
       
        event EventHandler SessionClosed;
    }
}
