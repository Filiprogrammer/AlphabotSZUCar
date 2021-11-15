using System;
using System.Collections.Generic;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Core.Handler
{
    public class ObstacleHandler
    {
        private List<Obstacle> _obstacles = new List<Obstacle>();

        public IReadOnlyCollection<Obstacle> Obstacles {
            get { return _obstacles.AsReadOnly(); }
        }

        public void AddObstacle(Obstacle obstacle)
        {
            _obstacles.Add(obstacle);
        }

        public bool RemoveObstacle(Obstacle obstacle)
        {
            return _obstacles.Remove(obstacle);
        }
    }
}
