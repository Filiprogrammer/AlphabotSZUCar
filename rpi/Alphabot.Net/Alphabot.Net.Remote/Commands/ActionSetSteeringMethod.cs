using Alphabot.Net.Car.Settings;

namespace Alphabot.Net.Remote.Commands
{
    internal class ActionSetSteeringMethod : AlphabotAction
    {
        Prefs _prefs = Prefs.GetInstance();
        public ActionSetSteeringMethod(string request) : base(request)
        {
        }

        public override void Perform()
        {
            //active steering
            // ass on / off

            if (Args.Length >= 1)
            {
                if (Args[0].ToLower().Contains("on"))
                {
                    _prefs.DeviceSettings.ActiveSteering = true;
                }
                if (Args[0].ToLower().Contains("off"))
                {
                    _prefs.DeviceSettings.ActiveSteering = false;
                }
            }
            ActionResult = "ok - active steering set to: " + _prefs.DeviceSettings.ActiveSteering;

        }
    }
}
