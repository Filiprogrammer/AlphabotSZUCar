namespace Alphabot.Net.Remote.Commands
{
    class ActionSetCollisionPrevention : AlphabotAction
    {
        public ActionSetCollisionPrevention(string request) : base(request)
        {
        }

        public override void Perform()
        {
            string result = "ok - collision prevention set to ";
            if (Args.Length >= 1)
            {
                if (Args[0].ToLower().Contains("on"))
                {
                    _car.StartCollisionPrevention();
                    result += "on";
                }
                if (Args[0].ToLower().Contains("off"))
                {
                    _car.StopCollisionPrevention();
                    result += "off";
                }
            }
            ActionResult = result;

        }
    }
}
