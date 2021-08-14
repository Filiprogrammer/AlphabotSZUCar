using Alphabot.Net.Car;
using Alphabot.Net.Shared.Logger;
namespace Alphabot.Net.Remote.Commands
{
    internal class ActionForward: AlphabotAction
    {
        public ActionForward(string request) : base(request)
        {
        }

        public override void Perform()
        {
            _car.MoveForward();
            ActionResult = "ok";
            _logger.Log(LogLevel.Information, "AlphabotForwardAction.Perform done");
        }
    }
}
