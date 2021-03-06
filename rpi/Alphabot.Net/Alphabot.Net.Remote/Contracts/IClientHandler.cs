using System.Collections.Generic;

namespace Alphabot.Net.Remote.Contracts
{
    public interface IClientHandler
    {
        IReadOnlyList<ISessionHandler> Sessions { get; }
        void AcceptClients();
        void Stop();
    }
}
