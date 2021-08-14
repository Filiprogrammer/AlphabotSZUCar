using Alphabot.Net.Shared.Contracts;

namespace Alphabot.Net.Shared.Logger
{
    public class ServiceLogger
    {
        IServiceLogger _serviceLogger;
        static readonly ServiceLogger _instance = new ServiceLogger();

        public IServiceLogger Current { get => _serviceLogger; set => _serviceLogger = value; }

        private ServiceLogger()
        {
            _serviceLogger = new ConsoleLogger();
        }
        public static ServiceLogger GetInstance()
        {
            return _instance;
        }
    }
}
