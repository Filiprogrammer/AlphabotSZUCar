using AlphabotClientLibrary.Core;
using AlphabotClientLibrary.Core.Tcp;
using AlphabotClientLibrary.Shared.Contracts;
using AlphabotClientLibrary.Shared.Models;
using AlphabotClientLibrary.Shared.Requests;
using AlphabotClientLibrary.Shared.Responses;
using System;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AlphabotXamarinClient
{
    public partial class MainPage : ContentPage
    {
        ConnectionHandler ch;

        bool isConnected;

        private GyroControl gyro;
        private double deviceWidth;
        private double deviceHeight;
        private StackOrientation deviceOrientation;

        DateTime _lastSteerMoveTime;
        int curSpeed = 0;
        int curSteer = 0;

        public MainPage()
        {
            InitializeComponent();

            ch = new TcpHandlerWindows();

            ch.ResponseHandler.AddResponseListener(ReceiveEvent);

            gyro = new GyroControl();

            gyro.ChangeDirectionLandscape += Gyro_ChangeDirectionLandscape;
            gyro.ChangeDirectionPortrait += Gyro_ChangeDirectionPortrait;
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

        private void CheckBox_GyroCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (cbxGyro.IsEnabled)
            {
                if (!gyro.ToggleGyroscope())
                {
                    DisplayAlert("Information", "This device does not support gyroscope features.", "Ok");
                    cbxGyro.IsChecked = false;
                    cbxGyro.IsEnabled = false;
                }
            }
        }

        private void CheckBoxTriggered()
        {
            ToggleRequest toggleRequest = new ToggleRequest(0);

            toggleRequest.DoCollisionAvoidance = false;
            toggleRequest.DoPositioningSystem = cbxDoPositioningsys.IsChecked;
            toggleRequest.LogPositioning = cbxLogPositioningsys.IsChecked;

            if (ch == null)
                return;

            ch.SendAction(toggleRequest);
        }

        private void Button_Calibrate_Clicked(object sender, EventArgs e)
        {
            ch.SendAction(new CalibrateSteeringRequest());
        }

        private void Button_AddSpeedClicked(object sender, EventArgs e)
        {
            curSpeed += 3;

            lblSpeedSteer.Text = "speed: " + curSpeed + ", steer: " + curSteer;
            ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
        }

        private void Button_RemoveSpeedClicked(object sender, EventArgs e)
        {
            curSpeed -= 3;

            lblSpeedSteer.Text = "speed: " + curSpeed + ", steer: " + curSteer;
            ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
        }

        private void Button_SteerLeftClicked(object sender, EventArgs e)
        {
            curSteer -= 10;

            lblSpeedSteer.Text = "speed: " + curSpeed + ", steer: " + curSteer;
            ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
        }

        private void Button_SteerRightClicked(object sender, EventArgs e)
        {
            curSteer += 10;

            lblSpeedSteer.Text = "speed: " + curSpeed + ", steer: " + curSteer;
            ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
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
            lblGyro.IsVisible = areMainButtonsVisible;
            cbxDoPositioningsys.IsVisible = areMainButtonsVisible;
            cbxLogPositioningsys.IsVisible = areMainButtonsVisible;
            cbxGyro.IsVisible = areMainButtonsVisible;
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

            lblSpeedSteer.IsVisible = areMainButtonsVisible;
            btnAddSpeed.IsVisible = areMainButtonsVisible;
            btnRemoveSpeed.IsVisible = areMainButtonsVisible;
            btnSteerLeft.IsVisible = areMainButtonsVisible;
            btnSteerRight.IsVisible = areMainButtonsVisible;
            lblPing.IsVisible = areMainButtonsVisible;
        }

        private void Button_SetPosAnchors_Clicked(object sender, EventArgs e)
        {
            ChangeVisibility(true, true);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width != this.deviceWidth || height != this.deviceHeight)
            {
                this.deviceWidth = width;
                this.deviceHeight = height;

                if (width > height)
                    deviceOrientation = StackOrientation.Horizontal;
                else
                    deviceOrientation = StackOrientation.Vertical;
            }
        }

        private void Gyro_ChangeDirectionLandscape(object sender, DirectionChangedEventArgs e)
        {
            if (deviceOrientation == StackOrientation.Horizontal)
                MoveWithGyro(e.Direction);
        }

        private void Gyro_ChangeDirectionPortrait(object sender, DirectionChangedEventArgs e)
        {
            if (deviceOrientation == StackOrientation.Vertical)
                MoveWithGyro(e.Direction);
        }

        private void MoveWithGyro(GyroDirection direction)
        {
            if (!isConnected)
                return;

            if ((DateTime.Now - _lastSteerMoveTime).TotalMilliseconds < 300 && direction != GyroDirection.Stop)
                return;

            switch (direction)
            {
                case GyroDirection.Left:
                    if (curSteer > -120)
                    {
                        curSteer -= 10;
                        lblSpeedSteer.Text = $"speed: {curSpeed }, steer: {curSteer}, GyroDir: {direction}";
                        ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
                    }
                    break;
                case GyroDirection.Right:
                    if (curSteer < 120)
                    {
                        curSteer += 10;
                        lblSpeedSteer.Text = $"speed: {curSpeed }, steer: {curSteer}, GyroDir: {direction}";
                        ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
                    }
                    break;
                case GyroDirection.Stop:
                    if (curSteer == 0) break;
                    curSteer = 0;
                    lblSpeedSteer.Text = $"speed: {curSpeed }, steer: {curSteer}, GyroDir: {direction}";
                    ch.SendAction(new SpeedSteerRequest(Convert.ToSByte(curSpeed), Convert.ToSByte(curSteer)));
                    break;
                default:
                    break;
            }
            _lastSteerMoveTime = DateTime.Now;
        }
    }
}
