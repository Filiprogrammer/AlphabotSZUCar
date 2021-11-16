
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
            this.btn_connect = new System.Windows.Forms.Button();
            this.tbx_port = new System.Windows.Forms.TextBox();
            this.tbx_ip = new System.Windows.Forms.TextBox();
            this.cbx_connected = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_disconnect = new System.Windows.Forms.Button();
            this.tbar_steer = new System.Windows.Forms.TrackBar();
            this.num_speed = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbl_steer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbar_steer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_speed)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(108, 187);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(112, 34);
            this.btn_connect.TabIndex = 0;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // tbx_port
            // 
            this.tbx_port.AccessibleDescription = "tbx_port";
            this.tbx_port.Location = new System.Drawing.Point(108, 119);
            this.tbx_port.Name = "tbx_port";
            this.tbx_port.Size = new System.Drawing.Size(150, 31);
            this.tbx_port.TabIndex = 1;
            this.tbx_port.Text = "9000";
            // 
            // tbx_ip
            // 
            this.tbx_ip.AccessibleDescription = "tbx_ip";
            this.tbx_ip.Location = new System.Drawing.Point(108, 51);
            this.tbx_ip.Name = "tbx_ip";
            this.tbx_ip.Size = new System.Drawing.Size(150, 31);
            this.tbx_ip.TabIndex = 2;
            this.tbx_ip.Text = "127.0.0.1";
            // 
            // cbx_connected
            // 
            this.cbx_connected.AutoSize = true;
            this.cbx_connected.Enabled = false;
            this.cbx_connected.Location = new System.Drawing.Point(668, 12);
            this.cbx_connected.Name = "cbx_connected";
            this.cbx_connected.Size = new System.Drawing.Size(120, 29);
            this.cbx_connected.TabIndex = 3;
            this.cbx_connected.Text = "connected";
            this.cbx_connected.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(108, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP-Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port:";
            // 
            // btn_disconnect
            // 
            this.btn_disconnect.Enabled = false;
            this.btn_disconnect.Location = new System.Drawing.Point(108, 228);
            this.btn_disconnect.Name = "btn_disconnect";
            this.btn_disconnect.Size = new System.Drawing.Size(112, 34);
            this.btn_disconnect.TabIndex = 6;
            this.btn_disconnect.Text = "Disconnect";
            this.btn_disconnect.UseVisualStyleBackColor = true;
            this.btn_disconnect.Click += new System.EventHandler(this.btn_disconnect_Click);
            // 
            // tbar_steer
            // 
            this.tbar_steer.Enabled = false;
            this.tbar_steer.LargeChange = 10;
            this.tbar_steer.Location = new System.Drawing.Point(314, 64);
            this.tbar_steer.Maximum = 125;
            this.tbar_steer.Minimum = -125;
            this.tbar_steer.Name = "tbar_steer";
            this.tbar_steer.Size = new System.Drawing.Size(316, 69);
            this.tbar_steer.SmallChange = 5;
            this.tbar_steer.TabIndex = 7;
            this.tbar_steer.Scroll += new System.EventHandler(this.tbar_steer_Scroll);
            // 
            // num_speed
            // 
            this.num_speed.Enabled = false;
            this.num_speed.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.num_speed.Location = new System.Drawing.Point(360, 190);
            this.num_speed.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_speed.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.num_speed.Name = "num_speed";
            this.num_speed.Size = new System.Drawing.Size(197, 31);
            this.num_speed.TabIndex = 8;
            this.num_speed.ValueChanged += new System.EventHandler(this.num_speed_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(314, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "LINKS";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(572, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 25);
            this.label4.TabIndex = 10;
            this.label4.Text = "RECHTS";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(433, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 25);
            this.label5.TabIndex = 11;
            this.label5.Text = "GERADE";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(360, 162);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(144, 25);
            this.label6.TabIndex = 12;
            this.label6.Text = "Geschwindigkeit:";
            // 
            // lbl_steer
            // 
            this.lbl_steer.AutoSize = true;
            this.lbl_steer.Location = new System.Drawing.Point(651, 85);
            this.lbl_steer.Name = "lbl_steer";
            this.lbl_steer.Size = new System.Drawing.Size(83, 25);
            this.lbl_steer.TabIndex = 13;
            this.lbl_steer.Text = "Lenkung:";
            // 
            // MainForm
            // 
            this.AccessibleDescription = "tbx_ip";
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbl_steer);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.num_speed);
            this.Controls.Add(this.tbar_steer);
            this.Controls.Add(this.btn_disconnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbx_connected);
            this.Controls.Add(this.tbx_ip);
            this.Controls.Add(this.tbx_port);
            this.Controls.Add(this.btn_connect);
            this.Name = "MainForm";
            this.Text = "connected";
            ((System.ComponentModel.ISupportInitialize)(this.tbar_steer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_speed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox tbx_ip;
        private System.Windows.Forms.TextBox tbx_port;
        private System.Windows.Forms.CheckBox cbx_connected;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_disconnect;
        private System.Windows.Forms.TrackBar tbar_steer;
        private System.Windows.Forms.NumericUpDown num_speed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbl_steer;
    }
}

