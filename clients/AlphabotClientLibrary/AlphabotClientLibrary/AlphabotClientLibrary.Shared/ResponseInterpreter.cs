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

            Array.Copy(DataBytes, timeBytes, 8);

            long time = (long)timeBytes[0] | ((long)timeBytes[1] << 8) |
                ((long)timeBytes[2] << 16) | ((long)timeBytes[3] << 24) |
                ((long)timeBytes[4] << 32) | ((long)timeBytes[5] << 40) |
                ((long)timeBytes[6] << 48) | ((long)timeBytes[7] << 56);

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

            if (DataBytes != null && DataBytes.Length >= 3)
            { 
                int len = DataBytes[2] & 0x3F;
                List<PathFindingResponse.PathFindingStep> steps = new List<PathFindingResponse.PathFindingStep>();

                for (int i = 0; i < len; ++i)
                {
                    int val = ((DataBytes[2 + (i * 3 + 6) / 8] & 0xFF) >> ((i * 3 + 6) % 8)) & 7;

                    if (((i * 3 + 6) % 8) > 5)
                        val |= ((DataBytes[3 + (i * 3 + 6) / 8] & 0xFF) << (8 - ((i * 3 + 6) % 8))) & 7;

                    steps.Add((PathFindingResponse.PathFindingStep) val);                  
                }

                return new PathFindingResponse(startByteX, startByteY, steps);
            }

            throw new ArgumentException("The Bytes doesn't fit the Path Finding Steps protocol definition.");
        }

        protected ErrorResponse GetErrorResponse()
        {
            byte[] packet = new byte[DataBytes.Length - 1];

            byte errorType = DataBytes[0];

            Array.Copy(DataBytes, 1, packet, 0, DataBytes.Length - 1);

            return new ErrorResponse((ErrorResponse.ErrorType)errorType, packet);
        }
    }
}
