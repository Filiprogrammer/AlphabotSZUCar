using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
