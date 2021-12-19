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
        protected readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;

        //TODO: Check if Factory works
    //    protected readonly IAlphabotCar _car = AlphabotCar.GetInstance();
        protected readonly IAlphabotCar _car = RemoteCar.GetInstance().Current;

        public int[] Args { get; set; }

        public AlphabotAction()
        {
            //TODO: Debug


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
