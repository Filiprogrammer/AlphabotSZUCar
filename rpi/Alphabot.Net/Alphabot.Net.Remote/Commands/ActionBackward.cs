using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Remote.Commands;

namespace Alphabot.Net.Remote.Commands

{
    internal class ActionBackward: AlphabotAction
    {
        public ActionBackward(string request) : base(request)
        {
        }

        public override void Perform()
        {
            _car.MoveBackward();
            ActionResult = "ok";
            _logger.Log(LogLevel.Information, "AlphabotBackwardAction.Perform: done");
        }
    }
}
