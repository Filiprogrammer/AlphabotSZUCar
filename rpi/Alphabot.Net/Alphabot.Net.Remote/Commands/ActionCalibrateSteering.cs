using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Remote.Core;

namespace Alphabot.Net.Remote.Commands
{
    internal class ActionCalibrateSteering: AlphabotAction
    {
        public ActionCalibrateSteering(string request) : base(request)
        {
        }

        public override void Perform()
        {
            _car.CalibrateActiveSteering();
            ActionResult = "ok";
            _logger.Log(LogLevel.Information, "AlphabotCalibrateSteeringAction.Perform done");
        }
    }
}
