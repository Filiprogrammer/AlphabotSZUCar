using System;

namespace AlphabotClientLibrary.Shared.Contracts
{
    public enum AlphabotResponseType
    {
        Compass,
        FrontDistanceSensor,
        BackDistanceSensor,
        Error,
        MultipleSensor,
        NewObstacle,
        PathFinding,
        Ping,
        Positioning,
        Toggle,
        AnchorDistances,
        WheelSpeed,
        Gyroscope,
        Accelerometer,
        Magnetometer
    }

    public interface IAlphabotResponse
    {
        AlphabotResponseType GetResponseType();
    }
}
