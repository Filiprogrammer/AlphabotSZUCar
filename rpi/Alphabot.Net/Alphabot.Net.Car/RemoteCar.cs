using System;
using System.Device.Gpio;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Car
{
    public class RemoteCar
    {
        private static RemoteCar _remoteCar = new RemoteCar();

        private readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;
        private readonly IAlphabotCar _currentCar;

        private RemoteCar()
        {
            _logger.Log(LogLevel.Debug, "ctor.RemoteCar", "check platform");
            if (SystemHasGpioController())
            {
                _currentCar = AlphabotCar.GetInstance();
                _logger.Log(LogLevel.Debug, "ctor.RemoteCar", "Platform ARM - has GPIO: AlphabotCar created");
            }
            else
            {
                _currentCar = new DummyCar();
                _logger.Log(LogLevel.Debug, "ctor.RemoteCar", "Platform has no GPIO: DummyCar created");
            }
        }

        public static RemoteCar GetInstance()
        {
            return _remoteCar;
        }

        public IAlphabotCar Current => _currentCar;

        private bool SystemHasGpioController()
        {
            var hasGpio = false;
            GpioController controller;
            _logger.Log(LogLevel.Debug, "ctor.RemoteCar", "testing gpio controller");
            try
            {
                controller = new GpioController();
                if (!controller.IsPinOpen(20)) controller.OpenPin(20, PinMode.Output);
                hasGpio = true;
                controller.ClosePin(20);
                _logger.Log(LogLevel.Debug, "ctor.RemoteCar", "testing gpio controller successfully");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "RemoteCar", $"No GPIO Controller found - dummyCar will be created\n{ex.Message}\n{ex.InnerException}");
            }

            return hasGpio;          
        }
    }
}
