namespace Alphabot.Net.Remote.Commands
{
    internal class ActionIdle : AlphabotAction
    {
        public ActionIdle(string request) : base(request)
        {
        }

        public override void Perform()
        {
            //idle
            ActionResult = "ok";

        }
    }
}
