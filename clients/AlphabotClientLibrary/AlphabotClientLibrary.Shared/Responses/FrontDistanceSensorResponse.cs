using System;
using AlphabotClientLibrary.Shared.Contracts;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class FrontDistanceSensorResponse : DistanceSensorResponse
    {
        public FrontDistanceSensorResponse(short degree, ushort distance) : base(degree, distance) {}

        public override AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.FrontDistanceSensor;
        }
    }
}
