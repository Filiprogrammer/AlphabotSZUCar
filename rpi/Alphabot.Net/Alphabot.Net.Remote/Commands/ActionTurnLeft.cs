namespace Alphabot.Net.Remote.Commands
{
    internal class ActionTurnLeft : AlphabotAction
    {
        public ActionTurnLeft(string request) : base(request)
        {
        }

        public override void Perform()
        {
            // string[] args = _request.Split(' ');
            int position = 57;
            if (Args.Length >= 1)
            {
                int.TryParse(Args[0], out position);
            }
            _car.TurnLeft(position);
            ActionResult = "ok";
        }
    }
}
