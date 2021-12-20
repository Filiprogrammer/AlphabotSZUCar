using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class AnchorDistancesResponse : IAlphabotResponse
    {
        public ushort DistanceAnchor0 { get; private set; }
        public ushort DistanceAnchor1 { get; private set; }
        public ushort DistanceAnchor2 { get; private set; }

        public AnchorDistancesResponse(ushort distanceAnchor0, ushort distanceAnchor1, ushort distanceAnchor2)
        {
            DistanceAnchor0 = distanceAnchor0;
            DistanceAnchor1 = distanceAnchor1;
            DistanceAnchor2 = distanceAnchor2;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.AnchorDistances;
        }
    }
}
