using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabotClientLibrary.Shared.Models
{
    public class BleInformation
    {
        public Guid Uuid { get; private set; }

        public byte[] Bytes { get; private set; }

        public BleInformation(Guid uuid, byte[] bytes)
        {
            Uuid = uuid;
            Bytes = bytes;
        }
    }
}
