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

            //time = BitConverter.ToInt64(timeBytes);
            time = timeBytes[0] | (timeBytes[1] << 8) |
                (timeBytes[2] << 16) | (timeBytes[3] << 24) |
                (timeBytes[4] << 32) | (timeBytes[5] << 40) |
                (timeBytes[6] << 48) | (timeBytes[7] << 56);

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

                    if (val == 4)
                        val = 8;

                    steps.Add((PathFindingResponse.PathFindingStep) val);                  
                }

                return new PathFindingResponse(startByteX, startByteY, steps);
            }

            throw new ArgumentException("The Bytes doesn't fit the Path Finding Steps protocol definition.");
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
