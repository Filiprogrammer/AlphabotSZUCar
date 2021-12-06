using AlphabotClientLibrary.Core;
using AlphabotClientLibrary.Core.Tcp;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;
using AlphabotClientLibrary.Shared.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlphabotClientLibrary.Example.WinForms
{
    public partial class MainForm : Form
    {
        ConnectionHandler _connectionHandler;
        sbyte _speed;
        sbyte _steer;
        sbyte _steerPuffer;

        public MainForm()
        {
            InitializeComponent();
            Thread thread = new Thread(UpdateSteer)
            {
                IsBackground = true
            };

            thread.Start();
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            _connectionHandler = new TcpHandlerWindows();

            IPAddress ip = IPAddress.Parse(tbxIp.Text);
            ushort port = Convert.ToUInt16(tbxPort.Text);

            if (_connectionHandler.Connect(new WiFiConnectionData(ip, port)))
            {
                cbxConnected.Checked = true;
                btnDisconnect.Enabled = true;
                btnConnect.Enabled = false;
                tbarSteer.Enabled = true;
                numSpeed.Enabled = true;
            }
            else
            {
                cbxConnected.Checked = false;
                btnConnect.Enabled = true;
            }
        }

        private void btn_disconnect_Click(object sender, EventArgs e)
        {
            _connectionHandler.Disconnect();
            cbxConnected.Checked = false;
            btnDisconnect.Enabled = false;
            btnConnect.Enabled = true;
            tbarSteer.Enabled = false;
            numSpeed.Enabled = false;
        }

        private void tbar_steer_Scroll(object sender, EventArgs e)
        {
            _steer = (sbyte)tbarSteer.Value;
            lblSteer.Text = "Lenkung: " + _steer;
        }

        private void num_speed_ValueChanged(object sender, EventArgs e)
        {
            _speed = (sbyte)numSpeed.Value;

            IAlphabotRequest request = new SpeedSteerRequest(_steer, _speed);
            _connectionHandler.SendAction(request);
        }

        private void UpdateSteer()
        {
            while (true)
            {
                Thread.Sleep(500);
                sbyte curSteer = _steer;
                if(_steerPuffer != curSteer)
                {
                    IAlphabotRequest req = new SpeedSteerRequest(curSteer, _speed);
                    _connectionHandler.SendAction(req);

                    _steerPuffer = curSteer;
                }
            }
        }
    }
}
