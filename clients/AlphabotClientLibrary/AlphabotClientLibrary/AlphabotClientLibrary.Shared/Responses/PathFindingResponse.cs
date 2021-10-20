using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

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

        public sbyte StartPositionX { get; private set; }
        public sbyte StartPositionY { get; private set; }
        public List<PathFindingStep> Steps { get; private set; }

        public PathFindingResponse(sbyte startPositionX, sbyte startPositionY, List<PathFindingStep> pathFindingSteps)
        {
            StartPositionX = startPositionX;
            StartPositionY = startPositionY;
            Steps = pathFindingSteps;
        }


    }
}
