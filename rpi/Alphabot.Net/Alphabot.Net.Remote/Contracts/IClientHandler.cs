using System.Collections.Generic;
using Alphabot.Net.Car.Contracts;

namespace Alphabot.Net.Remote.Contracts
{
    public interface IClientHandler
    {
        IReadOnlyList<ISessionHandler> Sessions { get; }
         void AcceptClients();
         void Stop();
    }
}
