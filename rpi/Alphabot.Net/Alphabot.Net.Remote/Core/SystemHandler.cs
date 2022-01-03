using System;
using Alphabot.Net.Car.Devices;

namespace Alphabot.Net.Remote.Core
{
    public class SystemHandler
    {
        private static SystemHandler _instance;

        public static SystemHandler GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SystemHandler();
                }
                return _instance;
            }
        }

        public PositioningSystem PositioningSystem { get; set; }
    }
}
