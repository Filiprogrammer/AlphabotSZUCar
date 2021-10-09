using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabotClientLibrary.Shared.Models
{
    public class Position
    {
        public short PositionX { get; private set; }
        public short PositionY { get; private set; }

        public Position(short x, short y)
        {
            PositionX = x;
            PositionY = y;
        }
    }
}
