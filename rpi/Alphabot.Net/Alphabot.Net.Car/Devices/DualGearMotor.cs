using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Car.Devices
{
    /// <summary>
    /// Controls the forward and backward movement and  speed of a dual gearMotor drive. Both connected to:
    /// H-bridge controller: L298N
    /// </summary>
    internal class DualGearMotor
    {
        private readonly GearMotor _leftMotor;
        private readonly GearMotor _rightMotor;
        readonly IServiceLogger _serviceLogger = ServiceLogger.GetInstance().Current;

        /// <summary>
        ///  Set indicated speed
        /// </summary>
        public int Speed
        {
            get => _leftMotor.Speed;
            set
            {
                _leftMotor.Speed = value;
                _rightMotor.Speed = value;
            }
        }

        /// <summary>
        /// Instance using two gearMotors which allows to in both directions an speedControl;
        /// </summary>
        public DualGearMotor(GearMotor rightMotor, GearMotor leftMotor)
        {
            _rightMotor = rightMotor;
            _leftMotor = leftMotor;
        }

        /// <summary>
        /// Makes a two motor drive stop moving
        /// </summary>
        public void Stop()
        {
            _leftMotor.Stop();
            _rightMotor.Stop();
            _serviceLogger.Log(LogLevel.Debug, "DualGearMotor:Stop", "motor stopped");
        }

        /// <summary>
        /// Makes a two motor drive moving forward with the configured speed
        /// </summary>
        public void MoveForward()
        {
            _leftMotor.MoveForward();
            _rightMotor.MoveForward();
            _serviceLogger.Log(LogLevel.Debug, "DualGearMotor:MoveForward", "motor running forward");
        }

        /// <summary>
        /// Makes a two motor drive moving forward with the configured speed
        /// </summary>
        public void MoveBackward()
        {
            _leftMotor.MoveBackward();
            _rightMotor.MoveBackward();
            _serviceLogger.Log(LogLevel.Debug, "DualGearMotor:MoveBackward", "motor running backward");
        }
    }
}
