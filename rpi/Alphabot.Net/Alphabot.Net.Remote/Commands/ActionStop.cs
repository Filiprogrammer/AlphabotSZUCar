namespace Alphabot.Net.Remote.Commands
{
    internal class ActionStop : AlphabotAction
    {
        public ActionStop(string request) : base(request)
        {
        }

        public override void Perform()
        {
            _car.Stop();
            ActionResult = "ok";
        }
    }
}
