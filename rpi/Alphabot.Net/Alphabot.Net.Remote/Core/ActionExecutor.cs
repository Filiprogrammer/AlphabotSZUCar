using Alphabot.Net.Remote.Commands;
using Alphabot.Net.Shared;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Shared.Requests;
using Alphabot.Net.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Alphabot.Net.Car.Devices.PositioningSystem;

namespace Alphabot.Net.Remote.Core
{
    public class ActionExecutor
    {
        IAlphabotRequest _req;
        private readonly IServiceLogger _serviceLogger = ServiceLogger.GetInstance().Current;
        private ResponseSender _responseSender;

        public ActionExecutor(IAlphabotRequest req, ResponseSender responseSender)
        {
            _req = req;
            _responseSender = responseSender;
        }

        public void Perform()
        {
            if (_req is SpeedSteerRequest)
            {
                PerformSpeedSteer();
            }
            else if (_req is CalibrateSteeringRequest)
            {
                PerformCalibrateSteering();
            }
            else if (_req is ConfigurePositioningAnchorRequest)
            {
                PerformConfigurePositioningAnchorRequest();
            }
            else if (_req is ToggleRequest)
            {
                PerformToggleRequest();
            }
            else if (_req is PingRequest)
            {
                PerformPingResponse();
            }
        }

        private void PerformPingResponse()
        {
            PingRequest pingRequest = _req as PingRequest;

            _responseSender(pingRequest.GetPingResponse());
        }

        private void PerformToggleRequest()
        {
            bool oldDoColAvoid = ToggleSettings.GetInstance.DoCollisionAvoidance;
            bool oldDoPosSys = ToggleSettings.GetInstance.DoPositioningSystem;
            bool oldLogPos = ToggleSettings.GetInstance.LogPositioning;

            ToggleRequest toggleRequest = _req as ToggleRequest;

            ToggleSettings.GetInstance.DoCollisionAvoidance = toggleRequest.DoCollisionAvoidance;
            ToggleSettings.GetInstance.DoPositioningSystem = toggleRequest.DoPositioningSystem;
            ToggleSettings.GetInstance.LogPositioning = toggleRequest.LogPositioning;

            _serviceLogger.Log(LogLevel.Debug, "ActionExecutor.PerformToggleRequest", "Toggle changes started.");

            if (oldLogPos != toggleRequest.LogPositioning)
            {
                _serviceLogger.Log(LogLevel.Debug, "ActionExecutor.PerformToggleRequest", "Positioning logging: " + toggleRequest.LogPositioning.ToString());
            }

            // Do CollisionAvoidance
            // dont do it because ultrasound sensor is augesteckt
            // new ActionSetCollisionPrevention(toggleRequest.DoCollisionAvoidance).Perform();

            // Do PositioningSystem
            if ((oldDoPosSys != toggleRequest.DoPositioningSystem) && toggleRequest.DoPositioningSystem)
            {
                _serviceLogger.Log(LogLevel.Debug, "ActionExecutor.PerformToggleRequest", "PositioningSystem activated");
                SystemHandler.GetInstance.PositioningSystem.Activate();
            }
            else if ((oldDoPosSys != toggleRequest.DoPositioningSystem) && !toggleRequest.DoPositioningSystem)
            {
                _serviceLogger.Log(LogLevel.Debug, "ActionExecutor.PerformToggleRequest", "PositioningSystem deactivated");
                SystemHandler.GetInstance.PositioningSystem.Deactivate();
            }
        }

        private void PerformConfigurePositioningAnchorRequest()
        {
            ConfigurePositioningAnchorRequest request = _req as ConfigurePositioningAnchorRequest;
            _serviceLogger.Log(LogLevel.Debug, "ActionExecutor.PerformConfigurePositioningAnchorRequest", "Set Anchor " + request.AnchorId + " to " + request.Position.PositionX + "X and " + request.Position.PositionY + "Y");
            SystemHandler.GetInstance.PositioningSystem.SetAnchorPosition(request.AnchorId, request.Position.PositionX, request.Position.PositionY);
        }

        private void PerformSpeedSteer()
        {
            SpeedSteerRequest speedSteerRequest = _req as SpeedSteerRequest;

            new ActionSetSpeed(speedSteerRequest._speed).Perform();
            new ActionTurn(speedSteerRequest._steer).Perform();
        }

        private void PerformCalibrateSteering()
        {
            new ActionCalibrateSteering().Perform();
        }
    }
}
