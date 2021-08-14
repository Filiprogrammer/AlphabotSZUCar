namespace Alphabot.Net.Car.Contracts
{
    public interface ISpeedController
    {
        /// <summary>
        ///  Set indicated speed
        /// </summary>
        int Speed { get; set; }
        void AddPin(int speedControlPin);
    }
}
