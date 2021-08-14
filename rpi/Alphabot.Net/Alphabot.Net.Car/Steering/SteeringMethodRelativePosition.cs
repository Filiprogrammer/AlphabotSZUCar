using System.Threading.Tasks;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Car.Steering
{
    public class SteeringMethodRelativePosition  : SteeringController, ISteeringMethod
    {
        int _currentPosition = 0;
        public SteeringMethodRelativePosition(int stepIn1, int stepIn2, int stepIn3, int stepIn4) : base(stepIn1, stepIn2, stepIn3, stepIn4)
        {

        }
        // TurnRightAsync
        public override async Task TurnRight(int relativPosition)
        {
            //check max relativ position
            if ((_currentPosition + relativPosition) > _prefs.DeviceSettings.CenterSteeringPosition)
            {
                relativPosition = _prefs.DeviceSettings.CenterSteeringPosition - _currentPosition;

            }
            _Logger.Log(LogLevel.Information, "SteeringStepper. TurnActiveRightRelativ: used right: " + relativPosition);

            await _steeringStepper.TurnAsync(relativPosition, TurnDirection.Right, CancellationTokenSource.Token);
            _currentPosition += relativPosition;
            _currentPosition = ValidateMaxSteeringPosition(_currentPosition);

            //await stepMotorDriver.Start(revolutions: 1, isClockwise: false, progress: null);
        }
        public override async Task TurnLeft(int relativPosition)
        {
            // check max relativ position
            if ((_currentPosition + (relativPosition * (-1)) < _prefs.DeviceSettings.CenterSteeringPosition * (-1)))
            {
                relativPosition = _prefs.DeviceSettings.CenterSteeringPosition + _currentPosition;

            }
            //TODO: Steuerung
            _Logger.Log(LogLevel.Information, "SteeringStepper. TurnLeftRelativ: used left: " + relativPosition);

            await _steeringStepper.TurnAsync(relativPosition, TurnDirection.Left, CancellationTokenSource.Token);
            _currentPosition -= relativPosition;
            _currentPosition = ValidateMaxSteeringPosition(_currentPosition);
        }
    }
}
