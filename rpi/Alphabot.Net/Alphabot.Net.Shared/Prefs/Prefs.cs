namespace Alphabot.Net.Car.Settings
{
    public class Prefs
    {
        static Prefs _instance = new Prefs();

        DeviceSettings _deviceSettings;
        ServiceSettings _serviceSettings;

        private Prefs()
        {
            _deviceSettings = new DeviceSettings();
            _serviceSettings = new ServiceSettings();
        }

        public DeviceSettings DeviceSettings { get => _deviceSettings; set => _deviceSettings = value; }
        public ServiceSettings ServiceSettings { get => _serviceSettings; set => _serviceSettings = value; }

        public static Prefs GetInstance()
        {
            return _instance;
        }
    }
}
