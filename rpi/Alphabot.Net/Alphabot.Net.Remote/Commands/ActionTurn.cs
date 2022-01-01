using System;

namespace Alphabot.Net.Remote.Commands
{
    internal class ActionTurn : AlphabotAction
    {
        public ActionTurn(int steer) : base()
        {
            Args = { steer };
        }

        public override void Perform()
        {
            int steer = Args[0];

            if (steer < 0)
            {
                _car.TurnLeft((int)Math.Round(steer * 2.28));
            }
            else if (steer > 0)
            {
                _car.TurnRight((int)Math.Round(steer * 2.28));
            }
            else
            {
                _car.CenterActiveSteering();
            }
        }
    }
}
