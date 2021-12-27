using System;
using System.Collections.Generic;
using System.Linq;
using AlphabotClientLibrary.Core.Tcp;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Requests;
using AlphabotClientLibrary.Shared.Responses;
using Xunit;

namespace AlphabotClientLibrary.Test
{
    public class TcpResponseTest
    {
        [Fact]
        public void TestCompassResponse()
        {
            byte[] bytes = { 0x04, 0x64, 0x00 };
            short expected = 100;

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is CompassResponse, "Response was of the wrong type");

            CompassResponse compassResponse = response as CompassResponse;
            Assert.Equal(expected, compassResponse.Degree);
        }

        [Fact]
        public void TestDistanceSensorResponse()
        {
            byte[] bytes = { 0x01, 0x02, 0x00, 0x02, 0x01 };
            short expectedDegree = 2;
            ushort expectedDistance = 258;

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is DistanceSensorResponse, "Response was of the wrong type");

            DistanceSensorResponse distanceSensorResponse = response as DistanceSensorResponse;
            Assert.Equal(expectedDegree, distanceSensorResponse.Degree);
            Assert.Equal(expectedDistance, distanceSensorResponse.Distance);
        }

        [Fact]
        public void TestErrorResponse()
        {
            byte[] bytes = { 0x08, 0x03, 0x0E, 0x01, 0x02, 0x03, 0x04, 0x05 };
            ErrorResponse.ErrorType expectedErrorType = ErrorResponse.ErrorType.UnknownPacketId;
            byte expectedHeader = 14;
            byte[] expectedPayload = { 0x01, 0x02, 0x03, 0x04, 0x05 };

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is ErrorResponse, "Response was of the wrong type");

            ErrorResponse errorResponse = response as ErrorResponse;
            Assert.Equal(expectedErrorType, errorResponse.Error);
            Assert.Equal(expectedHeader, errorResponse.Header);
            Assert.Equal(expectedPayload, errorResponse.Payload);
        }

        [Fact]
        public void TestNewObstacleRegisteredResponse()
        {
            byte[] bytes = { 0x06, 0x10, 0x10, 0x08, 0x08, 0x0A, 0x00, 0x14, 0x00, 0x05, 0x00 };
            short expectedXPos = 4112;
            short expectedYPos = 2056;
            ushort expectedWidth = 10;
            ushort expectedHeight = 20;
            ushort expectedId = 5;

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is NewObstacleRegisteredResponse, "Response was of the wrong type");

            NewObstacleRegisteredResponse newObstacleRegisteredResponse = response as NewObstacleRegisteredResponse;
            Assert.Equal(expectedXPos, newObstacleRegisteredResponse.Obstacle.Position.PositionX);
            Assert.Equal(expectedYPos, newObstacleRegisteredResponse.Obstacle.Position.PositionY);
            Assert.Equal(expectedWidth, newObstacleRegisteredResponse.Obstacle.Width);
            Assert.Equal(expectedHeight, newObstacleRegisteredResponse.Obstacle.Height);
            Assert.Equal(expectedId, newObstacleRegisteredResponse.Obstacle.Id);
        }

        [Fact]
        public void TestPathFindingResponse()
        {
            byte[] bytes = { 0x03, 0x05, 0x0A, 0x48, 0x94, 0x92, 0x1F };
            PathFindingResponse.PathFindingStep[] steps = {
                PathFindingResponse.PathFindingStep.Left,
                PathFindingResponse.PathFindingStep.LeftDown,
                PathFindingResponse.PathFindingStep.Left,
                PathFindingResponse.PathFindingStep.Down,
                PathFindingResponse.PathFindingStep.RightDown,
                PathFindingResponse.PathFindingStep.RightDown,
                PathFindingResponse.PathFindingStep.Right,
                PathFindingResponse.PathFindingStep.Up
            };
            sbyte expectedXPos = 5;
            sbyte expectedYPos = 10;
            List<PathFindingResponse.PathFindingStep> expectedSteps = steps.ToList();

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is PathFindingResponse, "Response was of the wrong type");

            PathFindingResponse pathFindingResponse = response as PathFindingResponse;
            Assert.Equal(expectedXPos, pathFindingResponse.StartPositionX);
            Assert.Equal(expectedYPos, pathFindingResponse.StartPositionY);
            Assert.Equal(expectedSteps, pathFindingResponse.Steps);
        }

        [Fact]
        public void TestPingResponse()
        {
            PingRequest request = new PingRequest();

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            byte[] bytes = new byte[9];
            bytes[0] = 5;
            Array.Copy(request.GetBytes(), 1, bytes, 1, 8);

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is PingResponse, "Response was of the wrong type");

            PingResponse pingResponse = response as PingResponse;
            long difference = pingResponse.Time - currentTime;
            Assert.True(difference >= 0 && difference < 1000, "The time difference was greater than 1000ms");
        }

        [Fact]
        public void TestPositioningResponse()
        {
            byte[] bytes = { 0x02, 0x96, 0x00, 0xFA, 0x00 };
            short expectedX = 150;
            short expectedY = 250;

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is PositioningResponse, "Response was of the wrong type");

            PositioningResponse positioningResponse = response as PositioningResponse;
            Assert.Equal(expectedX, positioningResponse.Position.PositionX);
            Assert.Equal(expectedY, positioningResponse.Position.PositionY);
        }

        [Fact]
        public void TestToggleResponse()
        {
            byte[] bytes = { 0x07, 0xC0, 0x30 };

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is ToggleResponse, "Response was of the wrong type");

            ToggleResponse toggleResponse = response as ToggleResponse;
            Assert.True(toggleResponse.DoInvite);
            Assert.True(toggleResponse.DoPositioningSystem);
            Assert.True(toggleResponse.LogPathfinderPath);
            Assert.True(toggleResponse.LogCompassDirection);
        }
    }
}
