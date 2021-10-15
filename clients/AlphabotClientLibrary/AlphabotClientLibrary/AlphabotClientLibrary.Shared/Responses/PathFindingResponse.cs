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
            leftUp = 0x00,
            left = 0x01,
            leftDown = 0x02,
            up = 0x03,
            rightDown = 0x04,
            down = 0x05,
            rightUp = 0x06,
            right = 0x07
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
