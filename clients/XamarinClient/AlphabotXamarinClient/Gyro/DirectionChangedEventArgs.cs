using System;
using Xamarin.Essentials;

namespace AlphabotXamarinClient
{
    public class DirectionChangedEventArgs : EventArgs
    {
        public GyroDirection Direction { get; set; }
        public GyroscopeData Data { get; set; }

        public DirectionChangedEventArgs(GyroDirection dir, GyroscopeData data)
        {
            Direction = dir;
            Data = data;
        }
    }
}
