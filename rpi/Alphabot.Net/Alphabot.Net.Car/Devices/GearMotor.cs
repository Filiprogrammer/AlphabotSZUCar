using System.Device.Gpio;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Car.Devices
{
    /// <summary>
    /// Controls the forward and backward movement of a single gearMotor (DC) connected to:
    /// H-bridge controller: L298N
    /// </summary>
    internal class GearMotor
    {
        private readonly GpioController _gpioController;
        private readonly int _motorControlPin1;
        private readonly int _motorControlPin2;
        
        private readonly ISpeedController _speedController;


        /// <summary>
        /// Instance using two pins which allows to in both directions;
        /// SpeedController based on PWM-Device Pca9685
        /// </summary>
        public GearMotor(int motorControlPin1, int motorControlPin2, ISpeedController speedController)
        {
            _motorControlPin1 = motorControlPin1;
            _motorControlPin2 = motorControlPin2;
            _speedController = speedController;

            _gpioController = new GpioController();

            InitializeControlPins();
            // set default configuration - stop motor
            Stop();
        }

        /// <summary>
        ///  Set indicated speed
        /// </summary>
        public int Speed
        {
            get => _speedController.Speed;
            set => _speedController.Speed = value;
        }

        /// <summary>
        /// Initializes controlPins - set pinMode to Output
        /// </summary>
        private void InitializeControlPins()
        {
            if (!_gpioController.IsPinOpen(_motorControlPin1))
                _gpioController.OpenPin(_motorControlPin1, PinMode.Output);

            if (!_gpioController.IsPinOpen(_motorControlPin2))
                _gpioController.OpenPin(_motorControlPin2, PinMode.Output);
        }

        /// <summary>
        /// Makes a single motor moving forward with the configured speed
        /// </summary>
        public void MoveForward()
        {
            _gpioController.Write(_motorControlPin1, PinValue.Low);
            _gpioController.Write(_motorControlPin2, PinValue.High);
        }

        /// <summary>
        /// Makes a single motor moving backwards with the configured speed
        /// </summary>
        public void MoveBackward()
        {
            _gpioController.Write(_motorControlPin1, PinValue.High);
            _gpioController.Write(_motorControlPin2, PinValue.Low);
        }

        /// <summary>
        /// Makes a single motor stop moving
        /// </summary>
        public void Stop()
        {
            _gpioController.Write(_motorControlPin1, PinValue.Low);
            _gpioController.Write(_motorControlPin2, PinValue.Low);
        }
    }
}
