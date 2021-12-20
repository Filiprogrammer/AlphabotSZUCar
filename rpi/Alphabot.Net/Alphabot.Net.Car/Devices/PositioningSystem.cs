using System;
using System.IO;
using System.IO.Ports;
using Alphabot.Net.Car.Contracts;
using Alphabot.Net.Shared;
using Alphabot.Net.Shared.Contracts;
using Alphabot.Net.Shared.Logger;
using Alphabot.Net.Shared.Responses;
using AlphabotClientLibrary.Shared.Contracts;

namespace Alphabot.Net.Car.Devices
{
    public class PositioningSystem
    {
        private ushort _anc1ShortAddress;
        private ushort _anc2ShortAddress;
        private ushort _anc3ShortAddress;
        private readonly IServiceLogger _logger = ServiceLogger.GetInstance().Current;

        public float Anc1Dist { get; private set; }

        public float Anc2Dist { get; private set; }

        public float Anc3Dist { get; private set; }

        public float Anc1X { get; private set; }

        public float Anc1Y { get; private set; }

        public float Anc2X { get; private set; }

        public float Anc2Y { get; private set; }

        public float Anc3X { get; private set; }

        public float Anc3Y { get; private set; }

        public SerialPort _serialPort;

        public delegate void ResponseSender(IAlphabotResponse response);

        private ResponseSender _responseSenderMethod;
        private readonly IAlphabotCar _car = RemoteCar.GetInstance().Current;

        public PositioningSystem(ushort anc1ShortAddress, ushort anc2ShortAddress, ushort anc3ShortAddress, ResponseSender responseSender)
        {
            if(_car is DummyCar)
            {
                // dont use positioning system if car is a dummy car
                return;
            }

            _anc1ShortAddress = anc1ShortAddress;
            _anc2ShortAddress = anc2ShortAddress;
            _anc3ShortAddress = anc3ShortAddress;
            _responseSenderMethod = responseSender;
            _serialPort = new SerialPort("/dev/ttyS1", 115200);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            try
            {
                _logger.Log(LogLevel.Debug, "PositioningSystem.ctor", "Opening SerialPort...");
                _serialPort.Open();
                _logger.Log(LogLevel.Debug, "PositioningSystem.ctor", "SerialPort opened");
            }
            catch (IOException ex)
            {
                _logger.Log(LogLevel.Error, "PositioningSystem.ctor", ex.InnerException.Message);
                _serialPort.Close();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Log(LogLevel.Error, "PositioningSystem.ctor", ex.InnerException.Message);
                _serialPort.Close();
            }
        }

        public void SetAnchorPosition(int ancId, float ancX, float ancY)
        {
            switch (ancId)
            {
                case 0:
                    Anc1X = ancX;
                    Anc1Y = ancY;
                    break;
                case 1:
                    Anc2X = ancX;
                    Anc2Y = ancY;
                    break;
                case 2:
                    Anc3X = ancX;
                    Anc3Y = ancY;
                    break;
                default:
                    throw new ArgumentException("AnchorId must be between 0 and 2");
            }
        }

        public Position CalculatePosition()
        {
            float[] P1 = { Anc1X, Anc1Y };
            float[] P2 = { Anc2X, Anc2Y };
            float[] P3 = { Anc3X, Anc3Y };

            float[] P2minusP1 = r2_subtract(P2, P1);
            float[] ex = r2_scale(P2minusP1, 1 / r2_norm(P2minusP1));
            float[] P3minusP1 = r2_subtract(P3, P1);
            float i = r2_dot(ex, P3minusP1);

            float[] exmuli = r2_scale(ex, i);
            float[] diff = r2_subtract(P3minusP1, exmuli);
            float[] ey = r2_scale(diff, 1 / r2_norm(diff));
            float d = r2_norm(P2minusP1);
            float j = r2_dot(ey, P3minusP1);

            float[] result = new float[2];
            result[0] = Anc1X + (sq(Anc1Dist) - sq(Anc2Dist) + sq(d)) / (2 * d);
            result[1] = Anc1X + ((sq(Anc1Dist) - sq(Anc3Dist) + sq(i) + sq(j)) / (2 * j)) - ((i / j) * result[0]);

            return new Position((short)Math.Round(result[0]), (short)Math.Round(result[1]));
        }

        private float[] r2_subtract(float[] v1, float[] v2)
        {
            float[] result = new float[2];
            result[0] = v1[0] - v2[0];
            result[1] = v1[1] - v2[1];
            return result;
        }

        private float[] r2_scale(float[] v, float r)
        {
            float[] result = new float[2];
            result[0] = r * v[0];
            result[1] = r * v[1];
            return result;
        }

        private float r2_dot(float[] v1, float[] v2)
        {
            return v1[0] * v2[0] + v1[1] * v2[1];
        }

        private float r2_norm(float[] v)
        {
            return (float)Math.Sqrt(sq(v[0]) + sq(v[1]));
        }

        private float sq(float v)
        {
            return v * v;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string fullLine = sp.ReadLine();

            if (fullLine != null)
                ProcessLine(fullLine);

            _responseSenderMethod(new PositioningResponse(CalculatePosition()));
        }

        private void ProcessLine(string line)
        {
            _logger.Log(LogLevel.Debug, "PositioningSystem.ProcessLine", $"Serial input: {line}");
            ushort shortAddress;

            if (line.Length == 0)
                return;

            switch (line[0])
            {
                case '+':
                    shortAddress = ushort.Parse(line.Substring(1, line.Length - 1));
                    _logger.Log(LogLevel.Debug, "PositioningSystem.ProcessLine", string.Format("DW1000 Device with short address {0:x16} connected", shortAddress));
                    break;
                case '-':
                    shortAddress = ushort.Parse(line.Substring(1, line.Length - 1));
                    _logger.Log(LogLevel.Debug, "PositioningSystem.ProcessLine", string.Format("DW1000 Device with short address {0:x16} disconnected", shortAddress));
                    break;
                case 'u':
                    string[] lineArgs = line.Split(':');

                    shortAddress = ushort.Parse(lineArgs[0].Substring(1, lineArgs[0].Length - 1), System.Globalization.NumberStyles.HexNumber);

                    if (shortAddress != _anc1ShortAddress && shortAddress != _anc2ShortAddress && shortAddress != _anc3ShortAddress)
                        break;

                    float range = float.Parse(lineArgs[1]);
                    range *= 100; // Convert from meters to centimeters

                    if (shortAddress == _anc1ShortAddress)
                        Anc1Dist = (float)((Anc1Dist + range) / 2.0);
                    else if (shortAddress == _anc2ShortAddress)
                        Anc2Dist = (float)((Anc2Dist + range) / 2.0);
                    else if (shortAddress == _anc3ShortAddress)
                        Anc3Dist = (float)((Anc3Dist + range) / 2.0);

                    _logger.Log(LogLevel.Debug, "PositioningSystem.ProcessLine", string.Format("DW1000 Device {0:x16} update range: {1}", shortAddress, range));
                    break;
            }
        }

        public void Deactivate()
        {
            _serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
            _serialPort.Close();

            _logger.Log(LogLevel.Debug, "PositioningSystem.Deactivate", "PositioningSystem deactivated");
        }

        public void Activate()
        {
            _serialPort.Open();
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        
            _logger.Log(LogLevel.Debug, "PositioningSystem.Activate", "PositioningSystem activated");
        }
    }
}
