using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Remote.Commands

{
    class ActionSetSpeed : AlphabotAction
    {
        public ActionSetSpeed(string request) : base(request)
        {
        }

        public override void Perform()
        {
   
            int speed = 0;
            if (_parser.GetArgs().Length >= 1)
            {
                int.TryParse(Args[0], out speed);
            }
            _logger.Log(LogLevel.Debug, "AlphabotAction", $"setspeed to:  {speed}");
            _car.SetSpeed(speed);
            ActionResult = "ok";
        }
    }
}
