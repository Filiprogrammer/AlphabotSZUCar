using System.Device.Pwm;

namespace Alphabot.Net.Car.Contracts
{
    public interface IPwmDevice
    {
        PwmChannel CreatePwmChannel(int channelPin);
        PwmChannel GetPwmChannel(int channelPin);
        double GetDutyCycle(int channelPin);
    }
}
