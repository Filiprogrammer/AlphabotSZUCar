using System;
using System.Threading.Tasks;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Car.Steering
{
    public class SteeringMethodAbsolutePosition:  SteeringController, ISteeringMethod
    {
        // DeviceSetting for absolute positioning
        // Alphabot.Raspi.Service.Shared.Settings.DeviceSettings.SteeringMethod =
        // SteeringMethod.AbsoluteSteering;


        public SteeringMethodAbsolutePosition(int stepIn1, int stepIn2, int stepIn3, int stepIn4) : base(stepIn1, stepIn2, stepIn3, stepIn4)
        {

        }

        public override async Task TurnRight(int absolutePosition)
        {
            CancelRunningSteeringActions();

            // only positive values
            absolutePosition = Math.Abs(ValidateMaxSteeringPosition(absolutePosition));
            if (absolutePosition == 0)
            {
                await CenterSteering();
                return;
            }
            TurnDirection requiredDirection = TurnDirection.Right; // default

            //     int requiredPosition = Math.Abs(_currentPosition - absolutePosition);
            int requiredPosition = Math.Abs(_steeringStepper.CurrentStepperPosition - absolutePosition);
            //if (absolutePosition < 0)
            //{
            //    requiredDirection = TurnDirection.Left;
            //}

            _Logger.Log(LogLevel.Information, "SteeringStepper. TurnRight absolut: used position: "+ requiredPosition);

            await _steeringStepper.TurnAsync(requiredPosition, requiredDirection, CancellationTokenSource.Token);
            // _currentPosition = absolutePosition;
            // _steeringStepper.SetCurrentStepperPosition(Math.Abs(absolutePosition));

        }

        public override async Task TurnLeft(int absolutePosition)
        {
            CancelRunningSteeringActions();

            absolutePosition = Math.Abs(ValidateMaxSteeringPosition(absolutePosition));

            if (absolutePosition == 0)
            {
                await CenterSteering();
                return;
            }
            absolutePosition *= (-1); // left direction
            
            int requiredPosition = Math.Abs(_steeringStepper.CurrentStepperPosition - absolutePosition);
            TurnDirection requiredDirection = TurnDirection.Right;

            if (absolutePosition < 0)
            {
                requiredDirection = TurnDirection.Left;
            }
            _Logger.Log(LogLevel.Debug, "SteeringStepper. TurnLeft absolut - Startposition:" + _steeringStepper.CurrentStepperPosition);
            _Logger.Log(LogLevel.Information, "SteeringStepper. TurnLeft absolut: used position: " + requiredPosition);

            await _steeringStepper.TurnAsync(requiredPosition, requiredDirection, CancellationTokenSource.Token);
            
            _Logger.Log(LogLevel.Debug, "SteeringStepper. TurnLeft absolut - Endposition: " + _steeringStepper.CurrentStepperPosition);

        }
    }
}
