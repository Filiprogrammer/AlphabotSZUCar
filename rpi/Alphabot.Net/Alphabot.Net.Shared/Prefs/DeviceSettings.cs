namespace Alphabot.Net.Car.Settings
{
    public class DeviceSettings
    {
        public DeviceSettings()
        {
            MotorRightPinA = 27;
            MotorRightPinB = 22;
            MotorLeftPinA = 5;
            MotorLeftPinB = 6;

            Pca9685PwmFrequency = 50;
            MotorMinimumSpeed = 15;
            MotorRightPca9685PwmPin = 0;
            MotorLeftPca9685PwmPin = 1;

            SteeringStepperIn1 = 13;
            SteeringStepperIn2 = 19;
            SteeringStepperIn3 = 26;
            SteeringStepperIn4 = 16;

            MinimumSteeringSteps = 12;
            CurrentSteeringMethod = 0;
            SteeringSpeed = 6; // Rpm

            ActiveSteering = true; // if false, passive steering active (3W - Alphabot)
            ActiveSteeringCalibrationOnStart = false;
            CenterSteeringPosition = 312; // Under perfect circumstances this would be 289.16, but use 312 to account for bendable plastic.

            CollisionPreventionFrontTriggerPin = 23;
            CollisionPreventionFrontEchoPin = 24;
            HoldOnCollisionAlarmCount = 3;
            MinimumDistanceToObstacle = 30; // cm
            MeasurementInterval = 1000; // in ms
        }


        #region DualGearMotor (drive)

        public int MotorRightPinA { get; set; }
        public int MotorRightPinB { get; set; }
        public int MotorLeftPinA { get; set; }
        public int MotorLeftPinB { get; set; }

        #endregion

        #region Motor (speed)
        public int Pca9685PwmFrequency{ get; set; }

        public int MotorMinimumSpeed { get; set; }
        public int MotorRightPca9685PwmPin { get; set; }

        public int MotorLeftPca9685PwmPin { get; set; }

        #endregion

        #region Stepper motor (steering)

        public int SteeringStepperIn1 { get; set; }
        public int SteeringStepperIn2 { get; set; }
        public int SteeringStepperIn3 { get; set; }
        public int SteeringStepperIn4 { get; set; }

        #endregion

        #region Active stepper steering

        public bool ActiveSteering { get; set; }
        public bool ActiveSteeringCalibrationOnStart { get; set; }
        public int CenterSteeringPosition { get; set; }

        public int MinimumSteeringSteps { get; set; }

        /// <summary>
        /// AbsoluteSteering = 0,
        /// RelativSteering = 1,
        /// None = 2,
        /// </summary>
        public int CurrentSteeringMethod { get; set; }

        public int SteeringSpeed { get; set; }


        public int MaxSteeringPosition
        {
            get
            {
                int tolerance = 5;
                return CenterSteeringPosition * 2 + tolerance;
            }
        }

        #endregion

        #region CollisionPrevention

        #region HC SR04
        public int CollisionPreventionFrontTriggerPin { get; set; }

        public int CollisionPreventionFrontEchoPin { get; set; }
        #endregion

        public int HoldOnCollisionAlarmCount { get; set; }

        public int MinimumDistanceToObstacle { get; set; }
        public int MeasurementInterval { get; set; }

        #endregion
    }
}
