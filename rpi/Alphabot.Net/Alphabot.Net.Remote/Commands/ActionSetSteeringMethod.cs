using System;
using Alphabot.Net.Car.Settings;

namespace Alphabot.Net.Remote.Commands
{
    internal class ActionSetSteeringMethod : AlphabotAction
    {
        Prefs _prefs = Prefs.GetInstance();
        public ActionSetSteeringMethod(bool isActiveSteering) : base()
        {
            Args = new int[] { Convert.ToInt32(isActiveSteering) };
        }

        public override void Perform()
        {
            //active steering
            // ass on / off

            if (Args.Length >= 1)
            {
                if (Args[0] == 1)
                {
                    _prefs.DeviceSettings.ActiveSteering = true;
                }
                if (Args[0] == 0)
                {
                    _prefs.DeviceSettings.ActiveSteering = false;
                }
            }
        }
    }
}
