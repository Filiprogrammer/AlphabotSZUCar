using Alphabot.Net.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphabot.Net.Remote.Core
{
    public class ToggleSettings
    {
        private static ToggleSettings _instance;
        public static ToggleSettings GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ToggleSettings();
                }
                return _instance;
            }
        }

        private ToggleSettings()
        {
            DoCollisionAvoidance = false;
            LogPositioning = true;
            DoPositioningSystem = true;
        }

        public bool DoCollisionAvoidance { get; set; }

        public bool LogPositioning { get; set; }

        public bool DoPositioningSystem { get; set; }

        public ToggleResponse GetToggleResponse()
        {
            ToggleResponse res = new ToggleResponse();

            res.DoCollisionAvoidance = DoCollisionAvoidance;
            res.LogPositioning = LogPositioning;
            res.DoPositioningSystem = DoPositioningSystem;

            return res;
        }
    }
}
