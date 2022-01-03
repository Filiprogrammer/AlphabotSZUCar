using System;

namespace Alphabot.Net.Remote.Commands
{
    class ActionSetCollisionPrevention : AlphabotAction
    {
        public ActionSetCollisionPrevention(bool preventCollision) : base()
        {
            Args = new int[] { Convert.ToInt32(preventCollision) };
        }

        public override void Perform()
        {
            if (Args.Length >= 1)
            {
                if (Args[0] == 1)
                {
                    // _car.StartCollisionPrevention();
                }
                if (Args[0] == 0)
                {
                    // _car.StopCollisionPrevention();
                }
            }
        }
    }
}
