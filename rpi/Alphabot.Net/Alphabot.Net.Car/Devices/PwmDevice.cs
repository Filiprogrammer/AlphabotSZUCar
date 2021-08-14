using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Device.Pwm;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Shared.Contracts;
using Iot.Device.Pwm;

namespace Alphabot.Net.Car.Devices
{
    internal class PwmDevice: IPwmDevice
    {
        private readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;
        private static readonly PwmDevice _instance = new PwmDevice();
        private readonly Pca9685 _pca9685;
        private readonly Dictionary<int, PwmChannel> _pwmChannels;
        private Prefs _prefs = Prefs.GetInstance();

        private PwmDevice()
        {
            _pwmChannels = new Dictionary<int, PwmChannel>();
            var settings = new I2cConnectionSettings(1, Pca9685.I2cAddressBase);

            try
            {
                _pca9685 = new Pca9685(I2cDevice.Create(settings));
                _pca9685.PwmFrequency = _prefs.DeviceSettings.Pca9685PwmFrequency; // 50

                _logger.Log(LogLevel.Error, "PwmFrequency set to: " + _pca9685.PwmFrequency);
            }
            catch (Exception)
            {
                _logger.Log(LogLevel.Warning, "Failed to create PCA9685 I2C device");
            }
        }

        public static PwmDevice GetInstance()
        {
            return _instance;
        }

        public PwmChannel CreatePwmChannel(int channelPin)
        {
            _logger.Log(LogLevel.Debug, "Try to create PCA9685 pwmChannel on Pin: " + channelPin);
            try
            {
                _pwmChannels.Add(channelPin, _pca9685.CreatePwmChannel(channelPin));
                _logger.Log(LogLevel.Information, "PCA9685 pwmChannel" + " created on pin " + channelPin);
            }
            catch (Exception)
            {
                _logger.Log(LogLevel.Warning,
                    "PCA9685 pwmChannel" + " failed to create on pin " + channelPin);
                return null;
            }

            return _pwmChannels[channelPin];
        }

        public PwmChannel GetPwmChannel(int channelPin)
        {
            PwmChannel channel = null;

            if (_pwmChannels.ContainsKey((channelPin)))
            {
                channel = _pwmChannels[channelPin];
            }
            else
            {
                _logger.Log(LogLevel.Warning,
                    "PCA9685 pwmChannel" + " on " + channelPin + " does not exist");
            }

            return channel;
        }

        public double GetDutyCycle(int channelPin)
        {
            if (_pwmChannels.ContainsKey(channelPin))
            {
                return _pca9685.GetDutyCycle(channelPin);
            }

            return 0;
        }
    }
}
