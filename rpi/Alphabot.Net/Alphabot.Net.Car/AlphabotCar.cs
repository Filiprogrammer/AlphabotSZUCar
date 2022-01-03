using System.Threading.Tasks;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Car.Devices;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Car.Steering;
using Alphabot.Net.Shared.Contracts;

namespace Alphabot.Net.Car
{
    /// <summary>
    /// Represents the Alphabot car with all controls and additional features
    /// </summary>
    public class AlphabotCar : IAlphabotCar
    {
        private readonly Prefs _prefs = Prefs.GetInstance();
        private readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;

        private readonly DualGearMotor _alphabotDrive;
        private readonly ISpeedController _speedController;

        //additional features
        private CollisionPreventionAssist _collisionPreventionAssist;
        private readonly ISteeringMethod _steeringMethod;
        private static readonly object _sync = new object();

       private static AlphabotCar _instance = null;
       //private static readonly AlphabotCar _instance = new AlphabotCar();

        public static AlphabotCar GetInstance()
        {
        
            lock (_sync)
            {
                if (_instance == null) // lazy init - check if platform has GPIO (RemoteCar)
                {
                    _instance = new AlphabotCar();
                }
            }
            return _instance;
        
        }

        private AlphabotCar()
        {
            _logger.Log(LogLevel.Debug, "AlphabotCar.Ctor: Begin");
            var motorRightPinA = _prefs.DeviceSettings.MotorRightPinA; // 27
            var motorRightPinB = _prefs.DeviceSettings.MotorRightPinB; //22
            var motorLeftPinA = _prefs.DeviceSettings.MotorLeftPinA; // 5
            var motorLeftPinB = _prefs.DeviceSettings.MotorLeftPinB; // 6

            // Speed control or the 2 dc motors
            _logger.Log(LogLevel.Debug, "AlphabotCar.Ctor: creating SpeedController");
            _speedController = new AlphabotSpeedController();
            _speedController.AddPin(_prefs.DeviceSettings.MotorRightPca9685PwmPin); // right motor back (0)
            _speedController.AddPin(_prefs.DeviceSettings.MotorLeftPca9685PwmPin); // left motor back (1)
            _speedController.Speed = _prefs.DeviceSettings.MotorMinimumSpeed;
            _logger.Log(LogLevel.Debug, "AlphabotCar.Ctor: creating DualGearMotor");
            _alphabotDrive = new DualGearMotor(new GearMotor(motorRightPinA, motorRightPinB, _speedController),
                new GearMotor(motorLeftPinA, motorLeftPinB, _speedController)); 
         
            //additional features
    //        _collisionPreventionAssist = new CollisionPreventionAssist();

            //  select active steering method (absolute; relative)
            
            _logger.Log(LogLevel.Debug, "AlphabotCar.Ctor: creating concrete steering method");

            _logger.Log(LogLevel.Information, "AlphabotCar.Ctor: Setting Steering Method");
            // AbsoluteSteering = 0,
            // RelativSteering = 1,
            // None = 2,
            if (_prefs.DeviceSettings.CurrentSteeringMethod== (int)SteeringMethod.AbsoluteSteering)
            {
                _steeringMethod = new SteeringMethodAbsolutePosition(_prefs.DeviceSettings.SteeringStepperIn1,
                    _prefs.DeviceSettings.SteeringStepperIn2, _prefs.DeviceSettings.SteeringStepperIn3,
                    _prefs.DeviceSettings.SteeringStepperIn4);
                _logger.Log(LogLevel.Information, "AlphabotCar.Ctor: SteeringMethod - active / absolut");
            }

            if (_prefs.DeviceSettings.CurrentSteeringMethod== (int)SteeringMethod.RelativSteering)
            {
                _steeringMethod = new SteeringMethodRelativePosition(_prefs.DeviceSettings.SteeringStepperIn1,
                    _prefs.DeviceSettings.SteeringStepperIn2, _prefs.DeviceSettings.SteeringStepperIn3,
                    _prefs.DeviceSettings.SteeringStepperIn4);
                _logger.Log(LogLevel.Information, "AlphabotCar.Ctor: SteeringMethod - active / relative");
            }
        }


        /// <summary>
        /// Makes the alphabot stop moving
        /// </summary>
        public async Task Stop()
        {
            _alphabotDrive.Stop();
           if (_collisionPreventionAssist.SensorIsMeasuring)
           {
               await  _collisionPreventionAssist.Stop();
           }
        }

        /// <summary>
        /// Makes a alphabot drive moving forward with the configured speed (defaultSpeed 10)
        /// </summary>
        public void MoveForward()
        {
            if (_speedController.Speed < _prefs.DeviceSettings.MotorMinimumSpeed)
            {
                _speedController.Speed = _prefs.DeviceSettings.MotorMinimumSpeed;
            }
            _alphabotDrive.MoveForward();
        }

        /// <summary>
        /// Makes a alphabot drive moving backward  with the configured speed (defaultSpeed 10)
        /// </summary>
        public void MoveBackward()
        {
            if (_speedController.Speed < _prefs.DeviceSettings.MotorMinimumSpeed)
            {
                _speedController.Speed = _prefs.DeviceSettings.MotorMinimumSpeed;
            }
            _alphabotDrive.MoveBackward();
        }

        /// <summary>
        /// Performs a steering action of the alphabot drive - right turn
        /// </summary>
        public async void TurnRight(int position = 57)
        {
            await _steeringMethod.TurnRight(position);
        }

        /// <summary>
        /// Performs a steering action of the alphabot drive - left turn
        /// position: only with active steering - relative position
        /// </summary>
        public async void TurnLeft(int position = 57)
        {
            await _steeringMethod.TurnLeft(position);
        }

        /// <summary>
        /// Configures des current speed of the alphabot drive - initial speed ist set in prefs (24)
        /// /// </summary>
        public void SetSpeed(int value)
        {
            _speedController.Speed = value;
        }

        /// <summary>
        /// Calibrate the steering of the alphabot drive - turn max anglge right; CalibrationPosition (DeviceSettings) left
        /// /// </summary>
        public async void CalibrateActiveSteering()
        {
            await _steeringMethod.Calibrate();
        }

        /// <summary>
        /// Centers the steering of the alphabot drive - set currentPosition to 0 (straight)
        /// /// </summary>
        public async void CenterActiveSteering()
        {
            await _steeringMethod.CenterSteering();
        }

        /// <summary>
        /// Turns the Collision Prevention on (Ultrasonic Distance Sensor)
        /// </summary>
        public async void StartCollisionPrevention()
        {
            _collisionPreventionAssist = new CollisionPreventionAssist(this);
            await _collisionPreventionAssist.Start();
        }

        /// <summary>
        /// Turns the Collision Prevention off (Ultrasonic Distance Sensor)
        /// </summary>
        public async void StopCollisionPrevention()
        {
             await  _collisionPreventionAssist.Stop();
        }
    }
}
