using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Shared.Contracts
{
    public interface IServiceLogger
    {
        void Log(LogLevel logLevel, string message);
        void Log(LogLevel logLevel, string source, string message);
    }
}
