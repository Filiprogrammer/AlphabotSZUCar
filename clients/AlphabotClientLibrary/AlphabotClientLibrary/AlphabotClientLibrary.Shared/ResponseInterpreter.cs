using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;
using AlphabotClientLibrary.Shared.Responses;

namespace AlphabotClientLibrary.Shared
{
    public abstract class ResponseInterpreter
    {
        protected byte[] DataBytes { get; set; }

        public abstract IAlphabotResponse GetResponse();

        protected PingResponse GetPingResponse()
        {
            byte[] timeBytes = new byte[8];
            long time;

            Array.Copy(DataBytes, timeBytes, 8);

            time = BitConverter.ToInt64(timeBytes);

            return new PingResponse(time);
        }

        protected ToggleResponse GetToggleResponse()
        {
            byte[] toggleBytes = new byte[2];

            Array.Copy(DataBytes, toggleBytes, 2);

            return new ToggleResponse(toggleBytes);
        }

        protected NewObstacleRegisteredResponse GetNewObstacleRegisteredResponse()
        {
            byte[] obstacleBytes = new byte[10];

            Array.Copy(DataBytes, obstacleBytes, 10);

            return new NewObstacleRegisteredResponse(new Obstacle(obstacleBytes));
        }

        protected PathFindingResponse GetPathFindingResponse()
        {
            sbyte startByteX = (sbyte)DataBytes[0];
            sbyte startByteY = (sbyte)DataBytes[1];

            throw new NotImplementedException();

            return new PathFindingResponse(startByteX, startByteY, null);
        }

        protected ErrorResponse GetErrorResponse()
        {
            byte[] packet = new byte[DataBytes.Length];

            byte errorType = DataBytes[0];

            Array.Copy(DataBytes, packet, DataBytes.Length);

            return new ErrorResponse((ErrorResponse.ErrorType)errorType, packet);
        }
    }
}
