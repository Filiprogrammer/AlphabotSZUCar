using System;
using System.Collections.Generic;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class PathFindingResponse : IAlphabotResponse
    {
        public enum PathFindingStep
        {
            LeftUp = 0x00,
            Left = 0x01,
            LeftDown = 0x02,
            Up = 0x03,
            RightDown = 0x04,
            Down = 0x05,
            RightUp = 0x06,
            Right = 0x07
        }

        public sbyte StartPositionX { get; }

        public sbyte StartPositionY { get; }

        public IReadOnlyCollection<PathFindingStep> Steps { get; }

        public PathFindingResponse(sbyte startPositionX, sbyte startPositionY, IReadOnlyCollection<PathFindingStep> pathFindingSteps)
        {
            StartPositionX = startPositionX;
            StartPositionY = startPositionY;
            Steps = pathFindingSteps;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.PathFinding;
        }
    }
}
