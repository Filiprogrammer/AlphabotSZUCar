using Alphabot.Net.Car;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Remote.Contracts;
using Alphabot.Net.Remote.Core;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Remote.Commands
{
    public abstract class AlphabotAction : IAlphabotAction
    {
       // private string[] _args;
      //  private string _command = string.Empty;
        protected string _request;

        protected readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;

        //TODO: Check if Factory works
    //    protected readonly IAlphabotCar _car = AlphabotCar.GetInstance();
        protected readonly IAlphabotCar _car = RemoteCar.GetInstance().Current;

        public string Command
        {
            get => _parser.GetCommand();
        }

        public string[] Args
        {
            get => _parser.GetArgs();
        }

        public string ActionResult { get; protected set; }

        protected  IProtocolParser _parser;

        public AlphabotAction(string request)
        {
            //TODO: Debug

            _request = request;
            _logger.Log(LogLevel.Debug, "AlphabotAction", $"REQUEST = {request}");
            _parser = new ProtocolParser(request);
            if (_car is AlphabotCar)
            {
                _logger.Log(LogLevel.Debug, "ctor: AlphabotAction","RC is AlphabotCar");
            }
            else
            {
                _logger.Log(LogLevel.Debug, "RC is DummyCar");
            }
        }

        public abstract void Perform();
    }
}
