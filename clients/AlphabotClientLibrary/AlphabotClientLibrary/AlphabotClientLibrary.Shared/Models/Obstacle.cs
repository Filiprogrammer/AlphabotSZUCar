using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabotClientLibrary.Shared.Models
{
    public class Obstacle
    {
        public byte Id { get; private set; }
        public Position Position { get; private set; }
        public ushort Width { get; private set; }
        public ushort Height { get; private set; }
    }
}
