using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Remote.Core;

namespace Alphabot.Net.Remote.Commands
{
    internal class ActionCenterSteering : AlphabotAction
    {
        public ActionCenterSteering() : base()
        {
        }

        public override void Perform()
        {
            _car.CenterActiveSteering();
            _logger.Log(LogLevel.Information, "AlphabotCenterSteeringAction.Perform done");
        }
    }
}
