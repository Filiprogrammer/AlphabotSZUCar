using System;
using Xamarin.Essentials;

namespace AlphabotXamarinClient
{
    public enum GyroDirection
    {
        Left,
        Right,
        Stop
    }

    public class GyroControl
    {
        readonly SensorSpeed speed = SensorSpeed.UI;
        readonly double _sensitivity;
        private GyroDirection lastDir = GyroDirection.Stop;

        public event EventHandler<GyroscopeChangedEventArgs> ReadingChanged;
        public event EventHandler<DirectionChangedEventArgs> ChangeDirectionPortrait;
        public event EventHandler<DirectionChangedEventArgs> ChangeDirectionLandscape;

        public GyroscopeData LastPos { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public float Z { get; private set; }

        public bool IsMonitoring { get { return Gyroscope.IsMonitoring; } }

        public GyroControl(double sensitivity = 5.5)
        {
            _sensitivity = sensitivity;
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
        }

        void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            if (!Gyroscope.IsMonitoring) return;
            GyroscopeData data = e.Reading;

            // Process Angular Velocity X, Y, and Z reported in rad/s 
            if (ReadingChanged != null)
                ReadingChanged(sender, e);

            X += data.AngularVelocity.X;
            Y += data.AngularVelocity.Y;
            Z += data.AngularVelocity.Z;

            CalcDirection(sender, data);
            LastPos = data;
        }

        private void CalcDirection(object sender, GyroscopeData data)
        {
            var y = Y + data.AngularVelocity.Y;
            var x = X + data.AngularVelocity.X;
            GyroscopeData gyroData = new GyroscopeData(X, Y, Z);
            if (ChangeDirectionPortrait != null)
            {
                double yRounded = Math.Round(y, 2);

                if (yRounded < _sensitivity * -1)
                {
                    ChangeDirectionPortrait(sender, new DirectionChangedEventArgs(GyroDirection.Left, gyroData));
                    lastDir = GyroDirection.Left;
                }
                else if (yRounded > _sensitivity)
                {
                    ChangeDirectionPortrait(sender, new DirectionChangedEventArgs(GyroDirection.Right, gyroData));
                    lastDir = GyroDirection.Right;
                }
                else if (lastDir != GyroDirection.Stop)
                {
                    ChangeDirectionPortrait(sender, new DirectionChangedEventArgs(GyroDirection.Stop, gyroData));
                    lastDir = GyroDirection.Stop;
                }
            }

            if (ChangeDirectionLandscape != null)
            {
                double xRounded = Math.Round(x, 2);

                if (xRounded < _sensitivity * -1)
                {
                    ChangeDirectionLandscape(sender, new DirectionChangedEventArgs(GyroDirection.Left, gyroData));
                    lastDir = GyroDirection.Left;
                }
                else if (xRounded > _sensitivity)
                {
                    ChangeDirectionLandscape(sender, new DirectionChangedEventArgs(GyroDirection.Right, gyroData));
                    lastDir = GyroDirection.Right;
                }
                else if (lastDir != GyroDirection.Stop)
                {
                    ChangeDirectionLandscape(sender, new DirectionChangedEventArgs(GyroDirection.Stop, gyroData));
                    lastDir = GyroDirection.Stop;
                }
            }
        }

        /// <summary>
        /// Toggles the gyroscope control
        /// </summary>
        /// <returns>true if the toggle was successful, false if the device does not support gyroscope features</returns>
        public bool ToggleGyroscope()
        {
            try
            {
                X = 0;
                Y = 0;
                Z = 0;
                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
                else
                    Gyroscope.Start(speed);

                return true;
            }
            catch (FeatureNotSupportedException)
            {
                return false;
            }
        }
    }
}
