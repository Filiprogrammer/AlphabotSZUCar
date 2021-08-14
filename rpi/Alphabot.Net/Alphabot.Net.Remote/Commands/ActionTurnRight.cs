namespace Alphabot.Net.Remote.Commands
{
    internal class ActionTurnRight : AlphabotAction
    {
        public ActionTurnRight(string request) : base(request)
        {
        }

        public override void Perform()
        {
            
            int position = 57;
            if (Args.Length >= 1)
            {
                int.TryParse(Args[0], out position);
            }
            _car.TurnRight(position);
            ActionResult = "ok";
        }
    }
}
