using System.Threading.Tasks;

namespace Alphabot.Net.Car.Contracts
{
    public interface IAlphabotCar
    {
        /// <summary>
        /// Makes the alphabot stop moving
        /// </summary>
        Task Stop();

        /// <summary>
        /// Makes a alphabot drive moving forward with the configured speed (defaultSpeed 10)
        /// </summary>
        void MoveForward();

        /// <summary>
        /// Makes a alphabot drive moving backward  with the configured speed (defaultSpeed 10)
        /// </summary>
        void MoveBackward();

        /// <summary>
        /// Performs a steering action of the alphabot drive - right turn
        /// </summary>
        void TurnRight(int position = 57);

        /// <summary>
        /// Performs a steering action of the alphabot drive - left turn
        /// position: only with active steering - relative position
        /// </summary>
        void TurnLeft(int position = 57);

        /// <summary>
        /// Configures des current speed of the alphabot drive - initial speed = 10%
        /// /// </summary>
        void SetSpeed(int value);

        /// <summary>
        /// Calibrate the steering of the alphabot drive - turn max position right; CalibrationPosition (DeviceSettings) left
        /// /// </summary>
        void CalibrateActiveSteering();

        /// <summary>
        /// Centers the steering of the alphabot drive - set currentPosition to 0 (straight)
        /// /// </summary>
        void CenterActiveSteering();

        /// <summary>
        /// Turns the Collision Prevention on (Ultrasonic Distance Sensor)
        /// </summary>
        void StartCollisionPrevention();

        /// <summary>
        /// Turns the Collision Prevention off (Ultrasonic Distance Sensor)
        /// </summary>
        void StopCollisionPrevention();
    }
}
