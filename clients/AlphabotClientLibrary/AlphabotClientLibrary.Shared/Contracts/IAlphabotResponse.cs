using System;

namespace AlphabotClientLibrary.Shared.Contracts
{
    public enum AlphabotResponseType
    {
        Compass,
        DistanceSensor,
        Error,
        MultipleSensor,
        NewObstacle,
        PathFinding,
        Ping,
        Positioning,
        Toggle
    }

    public interface IAlphabotResponse
    {
    }
}
