using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Remote.Commands

{
    class ActionSetSpeed : AlphabotAction
    {
        public ActionSetSpeed(int speed) : base()
        {
            Args = new int[] { speed };
        }

        public override void Perform()
        {
            int speed = Args[0];

            _logger.Log(LogLevel.Debug, "AlphabotAction", $"setspeed to:  {speed}");

            if (speed > 0)
            {
                _car.SetSpeed(speed);
                _car.MoveForward();
            }
            else if (speed < 0)
            {
                _car.SetSpeed(-speed);
                _car.MoveBackward();
            }
            else
            {
                _car.SetSpeed(0);
                _car.Stop();
            }
        }
    }
}
