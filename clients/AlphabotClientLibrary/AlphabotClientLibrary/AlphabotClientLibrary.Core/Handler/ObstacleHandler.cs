using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Core.Handler
{
    public class ObstacleHandler
    {
        public List<Obstacle> Obstacles { get; private set; }

        public void AddObstacle(Obstacle obstacle)
        {
            Obstacles.Add(obstacle);
        }

        public bool RemoveObstacle(Obstacle obstacle)
        {
            return Obstacles.Remove(obstacle);
        }

    }
}
