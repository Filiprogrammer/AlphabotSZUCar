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
        public void TestAccelerometerResponse()
        {
            float expectedX = 5.035351242f;
            float expectedY = -6.0001f;
            float expectedZ = 13.4f;
            byte[] xBytes = BitConverter.GetBytes(Convert.ToInt16(expectedX * 1000));
            byte[] yBytes = BitConverter.GetBytes(Convert.ToInt16(expectedY * 1000));
            byte[] zBytes = BitConverter.GetBytes(Convert.ToInt16(expectedZ * 1000));

            byte[] packetHeader = { 0x0B };
            byte[] responseBytes = packetHeader.Concat(xBytes).Concat(yBytes).Concat(zBytes).ToArray();

            IAlphabotResponse response = new TcpResponseInterpreter(responseBytes).GetResponse();
            Assert.True(response is AccelerometerResponse, "Response was of the wrong type");

            AccelerometerResponse accelerometerResponse = response as AccelerometerResponse;
            Assert.InRange(accelerometerResponse.XAxis, expectedX - 0.001, expectedX + 0.001);
            Assert.InRange(accelerometerResponse.YAxis, expectedY - 0.001, expectedY + 0.001);
            Assert.InRange(accelerometerResponse.ZAxis, expectedZ - 0.001, expectedZ + 0.001);
        }

        [Fact]
        public void TestAnchorDistancesResponse()
        {
            byte[] bytes = { 0x08, 0x6F, 0x00, 0xAE, 0x08, 0x35, 0x82 };
            ushort expectedDistance0 = 111;
            ushort expectedDistance1 = 2222;
            ushort expectedDistance2 = 33333;

            IAlphabotResponse response = new TcpResponseInterpreter(bytes).GetResponse();
            Assert.True(response is AnchorDistancesResponse, "Response was of the wrong type");

            AnchorDistancesResponse anchorDistanceResponse = response as AnchorDistancesResponse;
            Assert.Equal(expectedDistance0, anchorDistanceResponse.DistanceAnchor0);
            Assert.Equal(expectedDistance1, anchorDistanceResponse.DistanceAnchor1);
            Assert.Equal(expectedDistance2, anchorDistanceResponse.DistanceAnchor2);
        }

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
            byte[] bytes = { 0x0D, 0x03, 0x0E, 0x01, 0x02, 0x03, 0x04, 0x05 };
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
        public void TestGyroscopeResponse()
        {
            float expectedX = -24.298f;
            float expectedY = -8.0001f;
            float expectedZ = 1113.75f;
            byte[] xBytes = BitConverter.GetBytes(Convert.ToInt16(expectedX * 10));
            byte[] yBytes = BitConverter.GetBytes(Convert.ToInt16(expectedY * 10));
            byte[] zBytes = BitConverter.GetBytes(Convert.ToInt16(expectedZ * 10));

            byte[] packetHeader = { 0x0A };
            byte[] responseBytes = packetHeader.Concat(xBytes).Concat(yBytes).Concat(zBytes).ToArray();

            IAlphabotResponse response = new TcpResponseInterpreter(responseBytes).GetResponse();
            Assert.True(response is GyroscopeResponse, "Response was of the wrong type");

            GyroscopeResponse gyroscopeResponse = response as GyroscopeResponse;
            Assert.InRange(gyroscopeResponse.XAxis, expectedX - 0.1, expectedX + 0.1);
            Assert.InRange(gyroscopeResponse.YAxis, expectedY - 0.1, expectedY + 0.1);
            Assert.InRange(gyroscopeResponse.ZAxis, expectedZ - 0.1, expectedZ + 0.1);
        }

        [Fact]
        public void TestMagnetometerResponse()
        {
            float expectedX = 340.09f;
            float expectedY = -81.1f;
            float expectedZ = 78.751f;
            byte[] xBytes = BitConverter.GetBytes(Convert.ToInt16(expectedX * 10));
            byte[] yBytes = BitConverter.GetBytes(Convert.ToInt16(expectedY * 10));
            byte[] zBytes = BitConverter.GetBytes(Convert.ToInt16(expectedZ * 10));

            byte[] packetHeader = { 0x0C };
            byte[] responseBytes = packetHeader.Concat(xBytes).Concat(yBytes).Concat(zBytes).ToArray();

            IAlphabotResponse response = new TcpResponseInterpreter(responseBytes).GetResponse();
            Assert.True(response is MagnetometerResponse, "Response was of the wrong type");

            MagnetometerResponse magnetometerResponse = response as MagnetometerResponse;
            Assert.InRange(magnetometerResponse.XAxis, expectedX - 0.1, expectedX + 0.1);
            Assert.InRange(magnetometerResponse.YAxis, expectedY - 0.1, expectedY + 0.1);
            Assert.InRange(magnetometerResponse.ZAxis, expectedZ - 0.1, expectedZ + 0.1);
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

        [Fact]
        public void TestWheelSpeedResponse()
        {
            byte[] header = { 0x09 };
            float expectedSpeedLeft = 1.23f;
            float expectedSpeedRight = -0.19f;

            byte[] leftBytes = BitConverter.GetBytes(Convert.ToSByte(expectedSpeedLeft * 100));
            byte[] rightBytes = BitConverter.GetBytes(Convert.ToSByte(expectedSpeedRight * 100));

            byte[] responseByte = header.Concat(leftBytes).Concat(rightBytes).ToArray();

            IAlphabotResponse response = new TcpResponseInterpreter(responseByte).GetResponse();
            Assert.True(response is WheelSpeedResponse, "Response was of the wrong type");

            WheelSpeedResponse wheelSpeedResponse = response as WheelSpeedResponse;
            Assert.InRange(wheelSpeedResponse.SpeedLeft, expectedSpeedLeft - 0.2, expectedSpeedLeft + 0.2);
            Assert.InRange(wheelSpeedResponse.SpeedRight, expectedSpeedRight - 0.2, expectedSpeedRight + 0.2);
        }
    }
}
