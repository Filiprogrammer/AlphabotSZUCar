﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlphabotClientLibrary.Core.Handler;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;
using AlphabotClientLibrary.Shared.Responses;
using static AlphabotClientLibrary.Core.Handler.ResponseHandler;

namespace AlphabotClientLibrary.Core.Tcp
{
    public class TcpHandlerWindows : ConnectionHandler
    {
        private TcpClient _tcpClient;
        public TcpHandlerWindows(ResponseHandler responseHandler) : base(responseHandler) { }

        public TcpHandlerWindows() : base (new ResponseHandler()) { }
        public override bool Connect(IConnectionData connectionData)
        {
            if (!(connectionData is WiFiConnectionData))
            {
                throw new ArgumentException("connectionData must be of the type WiFiConnectionData");
            }

            base._connectionData = connectionData;
            WiFiConnectionData wiFiConnectionData = base._connectionData as WiFiConnectionData;

            _tcpClient = new TcpClient();
            _tcpClient.Connect(wiFiConnectionData.IPEndPoint);

            StartReadingThread();

            return _tcpClient.Connected;
        }

        public override void Disconnect()
        {
            _tcpClient.Close();
        }

        public override bool SendAction(IAlphabotRequest request)
        {
            if (_tcpClient == null || !_tcpClient.Connected || request == null)
            {
                return false;
            }

            _tcpClient.GetStream().Write(request.GetBytes());

            return true;
        }

        private void ReadData()
        {
            while (true)
            {
                byte[] data = new byte[1024];

                _tcpClient.GetStream().Read(data, 0, 1024);

                IAlphabotResponse alphabotResponse = new TcpResponseInterpreter(data).GetResponse();

                List<Response> responses = base.ResponseHandler.Listeners;
                
                foreach (Response response in responses)
                {
                    response(alphabotResponse);
                }
            }
        }
        
        private void StartReadingThread()
        {
            Thread thread = new Thread(ReadData);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}