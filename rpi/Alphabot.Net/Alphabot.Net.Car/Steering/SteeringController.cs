using System.Threading;
using System.Threading.Tasks;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Car.Devices;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Car.Steering
{
    /// <summary>
    /// Base Class to controls the steering wheels of the Alphabot
    /// </summary>
    public abstract class SteeringController
    {
        // protected int _currentSteeringStepsPosition = 0;

        // private int _stepIn1; //= 13;
        // private int _stepIn2; // = 19;
        // private int _stepIn3; // = 26;
        // private int _stepIn4; // = 16;

        protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        protected readonly Prefs _prefs = Prefs.GetInstance();
        protected readonly IServiceLogger _Logger = ServiceLogger.GetInstance().Current;

        protected readonly SteeringStepper _steeringStepper;

        public SteeringController(int stepIn1, int stepIn2, int stepIn3, int stepIn4)
        {

            _steeringStepper = new SteeringStepper(stepIn1, stepIn2, stepIn3, stepIn4);
        }


        public async Task Calibrate()
        {
            CancelRunningSteeringActions();
            //TODO: make DeviceSetting
            _steeringStepper.Rpm = (short) (_prefs.DeviceSettings.SteeringSpeed / 2);
            await _steeringStepper.TurnAsync(_prefs.DeviceSettings.MaxSteeringPosition, TurnDirection.Right, CancellationTokenSource.Token);
            await Task.Delay(500);
            await _steeringStepper.TurnAsync(_prefs.DeviceSettings.CenterSteeringPosition, TurnDirection.Left, CancellationTokenSource.Token);
            _steeringStepper.Rpm = (short)_prefs.DeviceSettings.SteeringSpeed;
            _steeringStepper.CurrentStepperPosition = 0;
            _Logger.Log(LogLevel.Debug, "Calibration done");
        }
        /// <summary>
        /// Brings the steering wheels to their configured neutral position
        /// </summary>
        public  async Task CenterSteering()
        {
            CancelRunningSteeringActions();

            if (_steeringStepper.CurrentStepperPosition > 0)
            {
                await _steeringStepper.TurnAsync(_steeringStepper.CurrentStepperPosition, TurnDirection.Left, CancellationTokenSource.Token);
            }
            else
            {
                await _steeringStepper.TurnAsync(_steeringStepper.CurrentStepperPosition * (-1), TurnDirection.Right, CancellationTokenSource.Token);
            }

            _steeringStepper.CurrentStepperPosition = 0;
            _Logger.Log(LogLevel.Debug, "CenterSteering done");

        }

        /// <summary>
        /// Turns the steering wheels to the right
        /// </summary>
        /// <param name="steps">Indicates the angle of the steering range the wheels should direct to.</param>
      //  public virtual async Task TurnRight(int steps)
        public virtual async Task TurnRight(int steps)
        {
            await Task.Delay(10);
        }

        /// <summary>
        /// Turns the steering wheels to the left
        /// </summary>
        /// <param name="anglePercentage">Indicates the percentage of the steering range the wheels should direct to.
        /// A value of 100 is full left, 50 is half left, etc.</param>
        /// <param name="steps"></param>
        public virtual async Task TurnLeft(int steps)
        {
            await Task.Delay(10);
        }

        public void Stop()
        {
            _steeringStepper.Stop();
        }
        
        /// <summary>
        /// Shuts down the stepper which controls the steering wheels
        /// </summary>
        protected void CancelRunningSteeringActions()
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource.Token.Register(_steeringStepper.Stop);
            //TODO: check if needed 
            Stop();
            _Logger.Log(LogLevel.Debug, "Uln 2003 - TurnAsync: Cancel:  ThrowIfCancellationRequested");
        }
        protected int ValidateMaxSteeringPosition(int valueToValidate)
        {
            if (valueToValidate > _prefs.DeviceSettings.CenterSteeringPosition)
            {
                valueToValidate = _prefs.DeviceSettings.CenterSteeringPosition;
            }
            if (valueToValidate < (_prefs.DeviceSettings.CenterSteeringPosition * (-1)))
            {
                valueToValidate = _prefs.DeviceSettings.CenterSteeringPosition * (-1);
            }

            _Logger.Log(LogLevel.Information, "SteeringStepper. ValidateMaxSteeringPosition: current: " + valueToValidate);
            return valueToValidate;
        }

    }
}
