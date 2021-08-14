using System;
using System.Threading.Tasks;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Car.Settings;
using Alphabot.Net.Shared.Contracts;
using Iot.Device.Hcsr04;
//using UnitsNet;

namespace Alphabot.Net.Car.Devices
{
    public class CollisionPreventionAssist
    {
        private readonly IServiceLogger _serviceLogger = ServiceLogger.GetInstance().Current;
        private  readonly Prefs _prefs = Prefs.GetInstance();

        private readonly double _minimumDistanceToObstacle;
        private readonly int _measurementInterval;
        private readonly int _holdOnCollisionAlarmCount;
        private int _collisionAlarmCount;
        
        //      SpeechService _speechService = SpeechService.GetInstance();

        private readonly Hcsr04 _hcsr04;
        private readonly IAlphabotCar _car;

        public CollisionPreventionAssist(IAlphabotCar car)
        {
            _car = car;
            SensorIsMeasuring = false;
            _holdOnCollisionAlarmCount = _prefs.DeviceSettings.HoldOnCollisionAlarmCount;
            _minimumDistanceToObstacle = _prefs.DeviceSettings.MinimumDistanceToObstacle;
            _measurementInterval = _prefs.DeviceSettings.MeasurementInterval;
            _hcsr04 = new Hcsr04(_prefs.DeviceSettings.CollisionPreventionFrontTriggerPin,
                _prefs.DeviceSettings.CollisionPreventionFrontEchoPin);
        }

        public bool SensorIsMeasuring { get; private set; }


        public async Task Stop()
        {
            SensorIsMeasuring = false;
            _serviceLogger.Log(LogLevel.Information, "CollisionPreventionAssist:Stop",
                ": stop successfully");
            //TODO: Check async
            await Task.Delay(10);
        }

        public async Task Start()
        {

            DateTime lastTalk = DateTime.Now;
            if (!SensorIsMeasuring)
            {
                _serviceLogger.Log(LogLevel.Information, "CollisionPreventionAssist.Start",
                    "begin measuring");
                SensorIsMeasuring = true;

                while (SensorIsMeasuring)
                {
                    await Task.Delay(_measurementInterval);

                    _hcsr04.TryGetDistance(out var distance);
                    
                    if (distance.Centimeters > _minimumDistanceToObstacle)
                    {
                        _serviceLogger.Log(LogLevel.Information, $"measured distance {distance} cm");
                        continue;
                    }
                    else
                    {
                        // false alarms?
                        if (distance.Centimeters != 0)
                        {
                            _collisionAlarmCount++;
                            if (_collisionAlarmCount++ >= _holdOnCollisionAlarmCount)
                            {
                                //TODO check if factory works
                               // await AlphabotCar.GetInstance().Stop();
                               await _car.Stop();
                               _collisionAlarmCount = 0;
                                _serviceLogger.Log(LogLevel.Information, $"Car stopped at {distance} cm or less. ");
                            }

                            //  
                            _serviceLogger.Log(LogLevel.Information, $"Obstacle found at {distance} cm or less. ");

                            #region TDOT

                            //TDOT - demo collision prevention - audible feedback
                            // TimeSpan timeSpan = DateTime.Now - lastTalk;
                            // if (timeSpan.TotalSeconds > _timespanAudibleWarnings)
                            // {
                            //     _speechService.Say("Hello - it's me- the collision prevention assistant - I have detected an obstacle at a distance of less than 20 cm.");
                            //     await Task.Delay(3500);
                            //     lastTalk = DateTime.Now;
                            // }

                            //end TDOT

                            #endregion
                        }
                        else
                        {
                            _serviceLogger.Log(LogLevel.Information, $"Obstacle found at {distance} cm or less. ");
                        }
                    }
                }
            }
        }
    }
}
