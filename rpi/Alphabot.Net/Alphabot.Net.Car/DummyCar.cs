using System.Threading.Tasks;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Shared.Contracts;

namespace Alphabot.Net.Car
{
    public class DummyCar:IAlphabotCar
    {
        
         private static readonly IAlphabotCar _alphabotCarImplementation = new DummyCar();
         private readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;
        public async Task Stop()
        {
            _logger.Log(LogLevel.Information,"DummyCar action: stop" );
            await Task.Delay(10);
        }

        public void MoveForward()
        {
            _logger.Log(LogLevel.Information,"DummyCar action: MoveForward" );
        }

        public void MoveBackward()
        {
            _logger.Log(LogLevel.Information,"DummyCar action: MoveBackward" );
        }

        public void TurnRight(int position = 57)
        {
            _logger.Log(LogLevel.Information,$"DummyCar action: TurnRight {position}" );
        }

        public void TurnLeft(int position = 57)
        {
            _logger.Log(LogLevel.Information,$"DummyCar action: TurnLeft {position}" );
        }

        public void SetSpeed(int value)
        {
            _logger.Log(LogLevel.Information,$"DummyCar action: SetSpeed {value}" );
        }

        public void CalibrateActiveSteering()
        {
            _logger.Log(LogLevel.Information,$"DummyCar action: CalibrateSteering" );
        }

        public void CenterActiveSteering()
        {
            _logger.Log(LogLevel.Information,$"DummyCar action: CenterSteering" );
        }

        public void StartCollisionPrevention()
        {
            _logger.Log(LogLevel.Information,$"DummyCar action: StartCollisionPrevention" );
        }

        public void StopCollisionPrevention()
        {
            _logger.Log(LogLevel.Information,$"DummyCar action: StopCollisionPrevention" );
        }
    }
}
