using System;
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

            byte[] expectedBytes = { 0x09, 0x28, 0x00, 0x1B, 0x29, 0x9F, 0x01, 0x65, 0x00 };
            Assert.Equal(request.GetBytes(), expectedBytes);
        }
    }
}
