using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphabotClientLibrary.Core.Handler;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;

namespace AlphabotClientLibrary.Core.Ble
{
    public class BleHandlerWindows : ConnectionHandler
    {
        private BleConnectionData _bleData;
        public BleHandlerWindows(ResponseHandler responseHandler) : base(responseHandler) { }

        public BleHandlerWindows() : base(new ResponseHandler()) { }
        public override bool Connect(IConnectionData connectionData)
        {
            if (!(connectionData is BleConnectionData))
            {
                throw new ArgumentException("connectionData must be of the type BleConnectionData");
            }

            base._connectionData = connectionData;
            _bleData = base._connectionData as BleConnectionData;

            //IBluetoothLowEnergyAdapter

            //IBluetoothLowEnergyAdapter ble = BluetoothLowEnergyAdapter.ObtainDefaultAdapter();
  // );

            StartReadingThread();

            return true;
        }

        public override void Disconnect()
        {
            //_tcpClient.Close();
        }

        public override bool SendAction(IAlphabotRequest request)
        {
            //if (_tcpClient == null || !_tcpClient.Connected || request == null)
            //{
                return false;
            //}

            //_tcpClient.GetStream().Write(request.GetBytes());

            return true;
        }

        private void ReadData()
        {
            while (true)
            {
                byte[] data = new byte[1024];

                //_tcpClient.GetStream().Read(data, 0, 1024);

                //IAlphabotResponse alphabotResponse = new TcpResponseInterpreter(data).GetResponse();

                //List<Response> responses = base.ResponseHandler.Listeners;

                //foreach (Response response in responses)
                //{
                //    response(alphabotResponse);
                //}
            }
        }

        private void StartReadingThread()
        {
            //Thread thread = new Thread(ReadData);
            //thread.IsBackground = true;
           // thread.Start();
        }
    }
}
