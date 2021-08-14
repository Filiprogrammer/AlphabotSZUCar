namespace Alphabot.Net.Car.Settings
{
    public class ServiceSettings
    {
        int _alphabotServicePort = 9000;

        public ServiceSettings()
        {
            CommandParamSeparator = ' ';
            SocketBufferSize = 1024;
        }

   

        public char CommandParamSeparator { get; set; }
        public int AlphabotServicePort { get => _alphabotServicePort; set => _alphabotServicePort = value; }
        
        public int SocketBufferSize { get; set; }
        
        

    }
}
