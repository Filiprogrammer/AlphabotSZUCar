using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Remote.Core;

namespace Alphabot.Net.Remote.Commands
{
    internal class ActionCalibrateSteering : AlphabotAction
    {
        public ActionCalibrateSteering() : base()
        {
        }

        public override void Perform()
        {
            _car.CalibrateActiveSteering();
            _logger.Log(LogLevel.Information, "AlphabotCalibrateSteeringAction.Perform done");
        }
    }
}
