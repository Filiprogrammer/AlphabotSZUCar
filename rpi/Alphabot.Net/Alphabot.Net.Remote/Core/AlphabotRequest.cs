using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Remote.Commands;
using Alphabot.Net.Remote.Contracts;

namespace Alphabot.Net.Remote.Core
{
    public class AlphabotRequest: IAlphabotRequest
    {
        private readonly string _request;

        public AlphabotRequest(string request)
        {
            _request = request;
        }
        
        public string ActionResult { get; private set; }

        public void PerformAction()
        {
            IAlphabotAction action = ActionFactory.CreateAction(_request);
            action.Perform();
            ActionResult = action.ActionResult;

        }
    }


}
