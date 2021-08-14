using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Remote.Contracts;
using Alphabot.Net.Remote.Core;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;

namespace Alphabot.Net.Remote.Commands
{
    public static class ActionFactory
    {
        static string _request = string.Empty;

        public static IAlphabotAction CreateAction(string request)
        {
            IServiceLogger _logger = ServiceLogger.GetInstance().Current;
            _request = request.ToLower().Trim();
            AlphabotAction result = null;
            IProtocolParser _parser = new ProtocolParser(request);
            string command = _parser.GetCommand();// request.Split(' ')[0].ToLower().Trim();

            switch (command)
            {
                case "forward":
                    result = new ActionForward(_request);
                    break;
                case "stop":
                    result = new ActionStop(_request);
                    break;
                case "backward":
                    result = new ActionBackward(_request);
                    break;
                case "right":
                    result = new ActionTurnRight(_request);
                    break;
                case "left":
                    result = new ActionTurnLeft(_request);
                    break;
                case "speed":
                    result = new ActionSetSpeed(_request);
                    break;
                case "collisionprevention":
                    result = new ActionSetCollisionPrevention(_request);
                    break;
                case "calibrate":
                    result = new ActionCalibrateSteering(_request);
                    break;
                case "center":
                    result = new ActionCenterSteering(_request);
                    break;
                case "say":
                    result = new ActionSay(_request);
                    break;

                default:
                    result = new ActionIdle(request);// does nothing
                    _logger.Log(LogLevel.Error, "ActionFactory.CreateAction", $"unknown Command {_request}");
                    break;
            }

            return result;
            
        }
    }
}
