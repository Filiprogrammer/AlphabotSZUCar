using System;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Shared.Responses
{
    public class NewObstacleRegisteredResponse : IAlphabotResponse
    {
        public Obstacle Obstacle { get; private set; }

        public NewObstacleRegisteredResponse(Obstacle obstacle)
        {
            Obstacle = obstacle;
        }

        public AlphabotResponseType GetResponseType()
        {
            return AlphabotResponseType.NewObstacle;
        }
    }
}
