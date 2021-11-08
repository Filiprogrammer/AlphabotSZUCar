using System;

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
