using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Models;
using AlphabotClientLibrary.Shared.Requests;
using Xunit;

namespace AlphabotClientLibrary.Test
{
    public class TcpRequestTest
    {
        [Fact]
        public void TestAddObstacleRequest()
        {
            Obstacle obstacle = new Obstacle(new Position(40, 10523), 415, 101);
            AddObstacleRequest request = new AddObstacleRequest(obstacle);

            byte[] expectedBytes = { 9, 40,0, 27, 41, 159, 1 , 101, 0};
            Assert.Equal(request.GetBytes(), expectedBytes);
        }
    }
}
