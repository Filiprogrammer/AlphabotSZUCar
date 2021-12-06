
namespace AlphabotClientLibrary.Example.WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbxPort = new System.Windows.Forms.TextBox();
            this.tbxIp = new System.Windows.Forms.TextBox();
            this.cbxConnected = new System.Windows.Forms.CheckBox();
            this.lblIp = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.tbarSteer = new System.Windows.Forms.TrackBar();
            this.numSpeed = new System.Windows.Forms.NumericUpDown();
            this.lblLeft = new System.Windows.Forms.Label();
            this.lblRight = new System.Windows.Forms.Label();
            this.lblCenter = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblSteer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbarSteer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(108, 187);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(112, 34);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // tbxPort
            // 
            this.tbxPort.Location = new System.Drawing.Point(108, 119);
            this.tbxPort.Name = "tbxPort";
            this.tbxPort.Size = new System.Drawing.Size(150, 31);
            this.tbxPort.TabIndex = 1;
            this.tbxPort.Text = "9000";
            // 
            // tbxIp
            // 
            this.tbxIp.Location = new System.Drawing.Point(108, 51);
            this.tbxIp.Name = "tbxIp";
            this.tbxIp.Size = new System.Drawing.Size(150, 31);
            this.tbxIp.TabIndex = 2;
            this.tbxIp.Text = "127.0.0.1";
            // 
            // cbxConnected
            // 
            this.cbxConnected.AutoSize = true;
            this.cbxConnected.Enabled = false;
            this.cbxConnected.Location = new System.Drawing.Point(668, 12);
            this.cbxConnected.Name = "cbxConnected";
            this.cbxConnected.Size = new System.Drawing.Size(120, 29);
            this.cbxConnected.TabIndex = 3;
            this.cbxConnected.Text = "connected";
            this.cbxConnected.UseVisualStyleBackColor = true;
            // 
            // lblIp
            // 
            this.lblIp.AutoSize = true;
            this.lblIp.Location = new System.Drawing.Point(108, 13);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(103, 25);
            this.lblIp.TabIndex = 4;
            this.lblIp.Text = "IP-Address:";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(108, 85);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(48, 25);
            this.lblPort.TabIndex = 5;
            this.lblPort.Text = "Port:";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(108, 228);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(112, 34);
            this.btnDisconnect.TabIndex = 6;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // tbarSteer
            // 
            this.tbarSteer.Enabled = false;
            this.tbarSteer.LargeChange = 10;
            this.tbarSteer.Location = new System.Drawing.Point(314, 64);
            this.tbarSteer.Maximum = 125;
            this.tbarSteer.Minimum = -125;
            this.tbarSteer.Name = "tbarSteer";
            this.tbarSteer.Size = new System.Drawing.Size(316, 69);
            this.tbarSteer.SmallChange = 5;
            this.tbarSteer.TabIndex = 7;
            this.tbarSteer.Scroll += new System.EventHandler(this.tbarSteer_Scroll);
            // 
            // numSpeed
            // 
            this.numSpeed.Enabled = false;
            this.numSpeed.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numSpeed.Location = new System.Drawing.Point(360, 190);
            this.numSpeed.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numSpeed.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.numSpeed.Name = "numSpeed";
            this.numSpeed.Size = new System.Drawing.Size(197, 31);
            this.numSpeed.TabIndex = 8;
            this.numSpeed.ValueChanged += new System.EventHandler(this.numSpeed_ValueChanged);
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(314, 51);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(58, 25);
            this.lblLeft.TabIndex = 9;
            this.lblLeft.Text = "LINKS";
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(572, 51);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(75, 25);
            this.lblRight.TabIndex = 10;
            this.lblRight.Text = "RECHTS";
            // 
            // lblCenter
            // 
            this.lblCenter.AutoSize = true;
            this.lblCenter.Location = new System.Drawing.Point(433, 51);
            this.lblCenter.Name = "lblCenter";
            this.lblCenter.Size = new System.Drawing.Size(78, 25);
            this.lblCenter.TabIndex = 11;
            this.lblCenter.Text = "GERADE";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(360, 162);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(144, 25);
            this.lblSpeed.TabIndex = 12;
            this.lblSpeed.Text = "Geschwindigkeit:";
            // 
            // lblSteer
            // 
            this.lblSteer.AutoSize = true;
            this.lblSteer.Location = new System.Drawing.Point(651, 85);
            this.lblSteer.Name = "lblSteer";
            this.lblSteer.Size = new System.Drawing.Size(83, 25);
            this.lblSteer.TabIndex = 13;
            this.lblSteer.Text = "Lenkung:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblSteer);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.lblCenter);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.numSpeed);
            this.Controls.Add(this.tbarSteer);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblIp);
            this.Controls.Add(this.cbxConnected);
            this.Controls.Add(this.tbxIp);
            this.Controls.Add(this.tbxPort);
            this.Controls.Add(this.btnConnect);
            this.Name = "MainForm";
            this.Text = "connected";
            ((System.ComponentModel.ISupportInitialize)(this.tbarSteer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox tbxIp;
        private System.Windows.Forms.TextBox tbxPort;
        private System.Windows.Forms.CheckBox cbxConnected;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.TrackBar tbarSteer;
        private System.Windows.Forms.NumericUpDown numSpeed;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.Label lblCenter;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblSteer;
    }
}

