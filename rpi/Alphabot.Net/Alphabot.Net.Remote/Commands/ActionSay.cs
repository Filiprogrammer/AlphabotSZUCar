using Alphabot.Net.Shared.Logger;
namespace Alphabot.Net.Remote.Commands
{
    internal class ActionSay : AlphabotAction
    {
     //   SpeechService _speechService = SpeechService.GetInstance();

        public ActionSay(string request) : base(request)
        {
        }

        public override void Perform()
        {
            string message = _request.Substring(3);
         //   _speechService.Say(message);
            _logger.Log(LogLevel.Information, "AlphabotSayAction.Perform not implemented");
        }
    }
}
