using System;
using System.Collections.Generic;
using System.Device.Pwm;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Shared.Contracts;

namespace Alphabot.Net.Car.Devices
{
    public class AlphabotSpeedController : ISpeedController
    {
        private readonly Prefs _prefs = Prefs.GetInstance();
        private readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;
        private readonly IPwmDevice _pwmDevice = PwmDevice.GetInstance();

        private readonly List<PwmChannel> _pwmChannels;

        public AlphabotSpeedController()
        {
            _pwmChannels = new List<PwmChannel>();

        }

        public void AddPin(int speedControlPin)
        {
           // if (_pwmDevice.GetPwmChannel(_prefs.DeviceSettings.MotorRightPca9685PwmPin) == null)
            if (_pwmDevice.GetPwmChannel(speedControlPin) == null)
            {
                PwmChannel pwmChannel = _pwmDevice.CreatePwmChannel(speedControlPin);

                if (pwmChannel != null)
                    _pwmChannels.Add(pwmChannel);

                _logger.Log(LogLevel.Information, "AlphabotSpeedController",
                    "channel created on pin: " + speedControlPin);
            }
            else
            {
                _logger.Log(LogLevel.Warning, "AlphabotSpeedController",
                    "channel on pin: " + speedControlPin + "is already in use.");
            }
        }

        /// <summary>
        /// Sets the movement speed of the alphabot.
        /// When the car is moving, its speed will instantly change to the given value.
        /// When the car is standing still, this method call will not affect the current movement speed.
        /// This Method is not able to stop the car since the minimum speed value is 22
        /// </summary>
        public int Speed
        {
            get
            {
                return Convert.ToInt32(_pwmDevice.GetDutyCycle(_prefs.DeviceSettings.MotorRightPca9685PwmPin) *
                                       100);
            }
            set
            {
                if (value < _prefs.DeviceSettings.MotorMinimumSpeed)
                {
                    value = _prefs.DeviceSettings.MotorMinimumSpeed;
                }

                foreach (var channel in _pwmChannels)
                {
                    // set speed
                    channel.DutyCycle = (double) value / 100;
                    _logger.Log(LogLevel.Debug,
                        "pwm dutyCycle set to " +
                        Convert.ToInt32(_pwmDevice.GetDutyCycle(_prefs.DeviceSettings.MotorRightPca9685PwmPin)));
                }

                _logger.Log(LogLevel.Information,
                    "speed set to " +
                    Convert.ToInt32(_pwmDevice.GetDutyCycle(_prefs.DeviceSettings.MotorRightPca9685PwmPin) * 100));
            }
        }
    }
}
