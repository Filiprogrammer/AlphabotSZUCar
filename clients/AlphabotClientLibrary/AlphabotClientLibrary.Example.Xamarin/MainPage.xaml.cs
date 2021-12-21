using AlphabotClientLibrary.Core;
using AlphabotClientLibrary.Core.Tcp;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;
using AlphabotClientLibrary.Shared.Requests;
using AlphabotClientLibrary.Shared.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AlphabotClientLibrary.Example.Xamarin
{
    public partial class MainPage : ContentPage
    {
        ConnectionHandler ch;

        bool isConnected;

        public MainPage()
        {
            InitializeComponent();

            ch = new TcpHandlerWindows();

            ch.ResponseHandler.AddResponseListener(ReceiveEvent);
        }

        private void HandlePingRequests()
        {
            while (isConnected)
            {
                Thread.Sleep(500);
                ch.SendAction(new PingRequest());
            }
        }

        private void ReceiveEvent(IAlphabotResponse res)
        {
            if (res is PositioningResponse)
            {
                PositioningResponse pres = res as PositioningResponse;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblXpos.Text = "X-pos: " + pres.Position.PositionX.ToString();
                    lblYpos.Text = "Y-pos: " + pres.Position.PositionY.ToString();
                });
            }
            else if (res is PingResponse)
            {
                PingResponse pres = res as PingResponse;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblPing.Text = "Last ping: " + pres.Latency + "ms";
                });
            }
        }

        private void Button_Connect_Clicked(object sender, EventArgs e)
        {
            string ip = entIp.Text;
            ushort port = Convert.ToUInt16(entPort.Text);
            bool isCon = ch.Connect(new WiFiConnectionData(ip, port));

            ChangeVisibility(true, false);

            if (isCon)
            {
                DisplayAlert("Information", "Connected", "Ok");
                isConnected = true;

                Thread pingThread = new Thread(HandlePingRequests);
                pingThread.Start();
            }
        }

        private void Button_SendPositioningAnchors_Clicked(object sender, EventArgs e)
        {
            ChangeVisibility(true, false);

            ch.SendAction(new ConfigurePositioningAnchorRequest(
                Convert.ToByte(entAncId.Text),
                new Position(Convert.ToInt16(entAncX.Text), Convert.ToInt16(entAncY.Text))
                ));
        }

        private void CheckBox_LogPositionCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBoxTriggered();
        }

        private void CheckBox_DoPositionCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBoxTriggered();
        }

        private void CheckBoxTriggered()
        {
            ToggleRequest toggleRequest = new ToggleRequest(0);

            toggleRequest.DoCollisionAvoidance = false;
            toggleRequest.DoPositioningSystem = cbxDoPositioningsys.IsChecked;
            toggleRequest.LogPositioning = cbxLogPositioningsys.IsChecked;

            if (ch == null)
            {
                return;
            }
            ch.SendAction(toggleRequest);
        }

        private void Button_Calibrate_Clicked(object sender, EventArgs e)
        {
            ch.SendAction(new CalibrateSteeringRequest());
        }

        int curSpeed = 0;
        int curSteer = 0;
        private void Button_Clicked_4(object sender, EventArgs e)
        {
            curSpeed += 3;

            lblSpeedSteer.Text = "speed: " + curSpeed + ", steer: " + curSteer;
            ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
        }

        private void Button_Clicked_5(object sender, EventArgs e)
        {
            curSpeed -= 3;

            lblSpeedSteer.Text = "speed: " + curSpeed + ", steer: " + curSteer;
            ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
        }

        private void Button_Clicked_6(object sender, EventArgs e)
        {
            curSteer -= 10;

            lblSpeedSteer.Text = "speed: " + curSpeed + ", steer: " + curSteer;
            ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
        }

        private void Button_Clicked_7(object sender, EventArgs e)
        {
            curSteer += 10;

            lblSpeedSteer.Text = "speed: " + curSpeed + ", steer: " + curSteer;
            ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
        }

        private void Button_Clicked_8(object sender, EventArgs e)
        {
            lblPing.Text = "Ping request started...";
            ch.SendAction(new PingRequest());
        }

        private void Button_Disconnect_Clicked(object sender, EventArgs e)
        {
            isConnected = false;
            ChangeVisibility(false, false);
            ch.Disconnect();
        }

        private void ChangeVisibility(bool areMainButtonsVisible, bool isConfigurePositioningAnchorsVisible)
        {
            lblIp.IsVisible = !areMainButtonsVisible;
            lblPort.IsVisible = !areMainButtonsVisible;
            entIp.IsVisible = !areMainButtonsVisible;
            entPort.IsVisible = !areMainButtonsVisible;
            btnConnect.IsVisible = !areMainButtonsVisible;

            btnDisconnect.IsVisible = areMainButtonsVisible;
            lblDoPos.IsVisible = areMainButtonsVisible;
            lblLogPos.IsVisible = areMainButtonsVisible;
            cbxDoPositioningsys.IsVisible = areMainButtonsVisible;
            cbxLogPositioningsys.IsVisible = areMainButtonsVisible;
            lblXpos.IsVisible = areMainButtonsVisible;
            lblYpos.IsVisible = areMainButtonsVisible;
            btnCalibrate.IsVisible = areMainButtonsVisible;

            btnSetPosAnchors.IsVisible = areMainButtonsVisible;
            btnSendAnchorPos.IsVisible = isConfigurePositioningAnchorsVisible;
            lblAncId.IsVisible = isConfigurePositioningAnchorsVisible;
            lblAncX.IsVisible = isConfigurePositioningAnchorsVisible;
            lblAncY.IsVisible = isConfigurePositioningAnchorsVisible;
            entAncId.IsVisible = isConfigurePositioningAnchorsVisible;
            entAncX.IsVisible = isConfigurePositioningAnchorsVisible;
            entAncY.IsVisible = isConfigurePositioningAnchorsVisible;

        }

        private void Button_SetPosAnchors_Clicked(object sender, EventArgs e)
        {
            ChangeVisibility(true, true);
        }
    }
}
