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

        [Fact]
        public void TestCalibrateCompassRequest()
        {
            CalibrateCompassRequest request = new CalibrateCompassRequest(CalibrateCompassRequest.CompassCalibrationType.StartManual);
            byte[] expectedBytes = { 0x04, 0x02 };

            Assert.Equal(expectedBytes, request.GetBytes());
        }

        [Fact]
        public void TestCalibrateSteeringRequest()
        {
            CalibrateSteeringRequest request = new CalibrateSteeringRequest();
            byte[] expectedBytes = { 0x03 };

            Assert.Equal(expectedBytes, request.GetBytes());
        }

        [Fact]
        public void TestConfigurePositioningAnchorRequest()
        {
            Position pos = new Position(-40, 390);
            ConfigurePositioningAnchorRequest request = new ConfigurePositioningAnchorRequest(1, pos);

            byte[] expectedBytes = { 0x07, 0x01, 0xD8, 0xFF, 0x86, 0x01 };

            Assert.Equal(expectedBytes, request.GetBytes());
        }

        [Fact]
        public void TestNavigationTargetRequest()
        {
            Position pos = new Position(10, 20);

            NavigationTargetRequest request = new NavigationTargetRequest(pos);

            byte[] expectedBytes = { 0x08, 0x0A, 0x00, 0x14, 0x00 };

            Assert.Equal(expectedBytes, request.GetBytes());
        }

        [Fact]
        public void TestPingRequest()
        {
            PingRequest request = new PingRequest();

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            byte[] requestArr = new byte[8];

            Array.Copy(request.GetBytes(), 1, requestArr, 0, 8);
            long requestTime = BitConverter.ToInt64(requestArr);

            long difference = requestTime - currentTime;

            Assert.True(difference >= 0 && difference < 1000, "The time difference was greater than 1000ms");
        }

        [Fact]
        public void TestRemoveAllObstaclesRequest()
        {
            RemoveAllObstaclesRequest request = new RemoveAllObstaclesRequest();
            byte[] expectedBytes = { 0x0C };

            Assert.Equal(expectedBytes, request.GetBytes());
        }

        [Fact]
        public void TestRemoveObstacleRequest()
        {
            RemoveObstacleRequest requestRemoveById = new RemoveObstacleRequest(257);
            byte[] expectedBytesRemoveById = { 0x0A, 0x01, 0x01 };

            //Test remove by ID
            Assert.Equal(expectedBytesRemoveById, requestRemoveById.GetBytes());
            
            Position pos = new Position(255, 2);
            RemoveObstacleRequest requestRemoveByPos = new RemoveObstacleRequest(pos);
            byte[] expectedBytesRemoveByPos = { 0x0B, 0xFF, 0x00, 0x02, 0x00 };

            //Test remove by position
            Assert.Equal(expectedBytesRemoveByPos, requestRemoveByPos.GetBytes());
        }

        [Fact]
        public void TestSpeedSteerRequest()
        {
            SpeedSteerRequest request = new SpeedSteerRequest(50, -1);
            byte[] expectedBytes = { 0x01, 0xFF, 0x32 };

            Assert.Equal(expectedBytes, request.GetBytes());
        }

        [Fact]
        public void TestToggleRequest()
        {
            ToggleRequest request = new ToggleRequest(0);
            request.DoExploreMode = true;
            request.LogPositioning = true;
            request.LogGyroscope = true;

            byte[] expectedBytes = { 0x05, 0x04, 0x81 };

            Assert.Equal(expectedBytes, request.GetBytes());
        }
    }
}
