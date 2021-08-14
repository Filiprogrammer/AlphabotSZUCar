using System.Threading.Tasks;

namespace Alphabot.Net.Car.Contracts
{
    public interface ISteeringMethod
    {
        Task CenterSteering();
        Task Calibrate();

        Task TurnRight(int absolutePosition);
        Task TurnLeft(int relativPosition);
        void Stop();
    }
}
