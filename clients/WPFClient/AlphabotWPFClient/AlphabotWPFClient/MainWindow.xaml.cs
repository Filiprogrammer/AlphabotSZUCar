using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;

namespace AlphabotWPFClient
{
    public struct Obstacle
    {
        public Obstacle(short x, short y, ushort width, ushort height, ushort id, bool selected = false)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Id = id;
            Selected = selected;
        }

        public short X { get; }
        public short Y { get; }
        public ushort Width { get; }
        public ushort Height { get; }
        public ushort Id { get; }
        public bool Selected { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;
        private string currentLine = string.Empty;
        private short lps_beacon0x;
        private short lps_beacon0y;
        private short lps_beacon1x;
        private short lps_beacon1y;
        private short lps_beacon2x;
        private short lps_beacon2y;
        private ushort frontDist;
        private ushort leftDist;
        private ushort rightDist;
        private ushort backDist;
        private ushort anchor1_dist;
        private ushort anchor2_dist;
        private ushort anchor3_dist;
        private short dir;
        private short lps_x;
        private short lps_y;
        private byte[] path;
        private short target_x;
        private short target_y;
        private bool has_target;
        private List<Obstacle> obstacles = new List<Obstacle>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SerialPortNames_DropDownOpened(object sender, EventArgs e)
        {
            serialPortNames.Items.Clear();

            foreach (string portName in SerialPort.GetPortNames())
                serialPortNames.Items.Add(portName);
        }

        private void SerialPortNames_SelectionChanged(object sender, RoutedEventArgs e)
        {
            serialPort?.Close();

            if (serialPortNames.SelectedValue != null)
            {
                serialPort = new SerialPort((string)serialPortNames.SelectedValue, 115200);
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                try
                {
                    serialPort.Open();
                    serialPort.WriteLine("anchors");
                }
                catch (IOException ex)
                {
                    serialPortNames.SelectedIndex = -1;
                    serialPort.Close();
                }
                catch (UnauthorizedAccessException ex)
                {
                    serialPortNames.SelectedIndex = -1;
                    serialPort.Close();
                }
            }
        }

        private void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string fullLine = null;

            lock (currentLine)
            {
                string str = sp.ReadLine();

                if (str.EndsWith("\r"))
                {
                    currentLine = string.Empty;
                    fullLine = currentLine + str.Substring(0, str.Length - 1);
                }
                else
                {
                    currentLine += str;
                }
            }

            if (fullLine != null)
                ProcessLine(fullLine);
        }

        private void ProcessLine(string line)
        {
            if (line.StartsWith("SENSOR: "))
            {
                string hex = line.Substring(8);
                byte[] bytes = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

                if (bytes.Length < 2)
                    return;

                byte[] sensorTypes = new byte[8];
                byte sensorCount = 8;

                for (byte i = 0; i < 8; ++i)
                {
                    byte sensorType = (byte)((bytes[i / 4] >> ((i % 4) * 2)) & 0x03);

                    if (sensorType == 0)
                        sensorCount = i;
                    else
                        sensorTypes[i] = sensorType;
                }

                byte offset = 2;

                for (byte i = 0; i < sensorCount; ++i)
                {
                    switch (sensorTypes[i])
                    {
                        case 1:
                            // Distance sensor
                            int direction = bytes[offset] * 2;
                            ushort distance = (ushort)(bytes[offset + 1] * 2);

                            if (direction == 0)
                                frontDist = distance;
                            else if (direction == 334)
                                leftDist = distance;
                            else if (direction == 180)
                                backDist = distance;
                            else if (direction == 24)
                                rightDist = distance;

                            offset += 2;
                            break;
                        case 2:
                            // Position
                            lps_x = (short)(bytes[offset] | ((bytes[offset + 1] & 0x0F) << 8));
                            lps_y = (short)(((bytes[offset + 1] & 0xF0) >> 4) | (bytes[offset + 2] << 4));
                            offset += 3;
                            break;
                        case 3:
                            // Compass
                            dir = (short)(bytes[offset] | (bytes[offset + 1] << 8));
                            offset += 2;
                            break;
                    }
                }

                if (sensorCount != 0)
                    RedrawMap();
            }
            else if (line.StartsWith("PATHFINDING_TARGET: "))
            {
                string hex = line.Substring(20);

                if (hex.Length >= 24)
                {
                    byte[] bytes = Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();

                    // Set target position
                    target_x = (short)(bytes[0] | (bytes[1] << 8));
                    target_y = (short)(bytes[2] | (bytes[3] << 8));
                    has_target = true;
                }
            }
            else if (line.StartsWith("ADD_OBSTACLE: "))
            {
                string hex = line.Substring(14);

                if (hex.Length == 36)
                {
                    byte[] bytes = Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();

                    short obstacle_x = (short)(bytes[8] | (bytes[9] << 8));
                    short obstacle_y = (short)(bytes[10] | (bytes[11] << 8));
                    ushort obstacle_w = (ushort)(bytes[12] | (bytes[13] << 8));
                    ushort obstacle_h = (ushort)(bytes[14] | (bytes[15] << 8));
                    ushort obstacle_id = (ushort)(bytes[16] | (bytes[17] << 8));
                    obstacles.Add(new Obstacle(obstacle_x, obstacle_y, obstacle_w, obstacle_h, obstacle_id));
                    RedrawMap();
                }
            }
            else if (line.StartsWith("REMOVE_OBSTACLE: "))
            {
                string hex = line.Substring(17);
                byte[] bytes = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

                if (bytes.Length == 0)
                {
                    obstacles.Clear();
                    RedrawMap();
                }
                else if (bytes.Length == 12)
                {
                    short x = (short)(bytes[8] | (bytes[9] << 8));
                    short y = (short)(bytes[10] | (bytes[11] << 8));
                    bool removed = false;

                    foreach (Obstacle o in obstacles)
                    {
                        if (x >= o.X && x <= (o.X + o.Width) && y >= o.Y && y <= (o.Y + o.Height))
                        {
                            obstacles.Remove(o);
                            removed = true;
                        }
                    }

                    if (removed)
                        RedrawMap();
                }
                else if (bytes.Length == 10)
                {
                    short id = (short)(bytes[8] | (bytes[9] << 8));

                    foreach (Obstacle o in obstacles)
                    {
                        if (id == o.Id)
                        {
                            obstacles.Remove(o);
                            RedrawMap();
                            break;
                        }
                    }
                }
            }
            else if (line.StartsWith("PATHFINDING_PATH: "))
            {
                string hex = line.Substring(18);
                path = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
                RedrawMap();
            }
            else if (line.StartsWith("ANCHOR_LOCATIONS: "))
            {
                string hex = line.Substring(18);

                if (hex.Length == 40)
                {
                    byte[] bytes = Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();

                    lps_beacon0x = (short)(bytes[8] | (bytes[9] << 8));
                    lps_beacon0y = (short)(bytes[10] | (bytes[11] << 8));
                    lps_beacon1x = (short)(bytes[12] | (bytes[13] << 8));
                    lps_beacon1y = (short)(bytes[14] | (bytes[15] << 8));
                    lps_beacon2x = (short)(bytes[16] | (bytes[17] << 8));
                    lps_beacon2y = (short)(bytes[18] | (bytes[19] << 8));
                }
            }
        }

        private void RedrawMap()
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                mapCanvas.Children.Clear();
                
                int minX = Math.Min(lps_beacon0x, Math.Min(lps_beacon1x, Math.Min(lps_beacon2x, lps_x))) - 5;
                int minY = Math.Min(lps_beacon0y, Math.Min(lps_beacon1y, Math.Min(lps_beacon2y, lps_y))) - 5;
                int maxX = Math.Max(lps_beacon0x, Math.Max(lps_beacon1x, Math.Max(lps_beacon2x, lps_x))) + 5;
                int maxY = Math.Max(lps_beacon0y, Math.Max(lps_beacon1y, Math.Max(lps_beacon2y, lps_y))) + 5;

                float zoom = (float)Math.Min(mapCanvas.ActualWidth / (maxX - minX), mapCanvas.ActualHeight / (maxY - minY));

                foreach (Obstacle obstacle in obstacles)
                {
                    Rectangle obstacleRect = new Rectangle();
                    obstacleRect.Stroke = obstacle.Selected ? Brushes.GreenYellow : Brushes.Red;
                    obstacleRect.Opacity = 0.75;
                    obstacleRect.Fill = new SolidColorBrush(obstacle.Selected ? Colors.GreenYellow : Colors.Red) { Opacity = 0.25 };
                    obstacleRect.Margin = new Thickness((obstacle.X - minX) * zoom, (obstacle.Y - minY) * zoom, 0, 0);
                    obstacleRect.Width = obstacle.Width * zoom;
                    obstacleRect.Height = obstacle.Height * zoom;
                    mapCanvas.Children.Add(obstacleRect);
                }

                Rectangle anchor1Rect = new Rectangle();
                anchor1Rect.Stroke = Brushes.Yellow;
                anchor1Rect.Margin = new Thickness((lps_beacon0x - 5 - minX) * zoom, (lps_beacon0y - 5 - minY) * zoom, 0, 0);
                anchor1Rect.Width = 10 * zoom;
                anchor1Rect.Height = 10 * zoom;
                mapCanvas.Children.Add(anchor1Rect);

                TextBlock anchor1TextBlock = new TextBlock();
                anchor1TextBlock.Foreground = Brushes.Magenta;
                Canvas.SetLeft(anchor1TextBlock, (lps_beacon0x - minX) * zoom - 3);
                Canvas.SetTop(anchor1TextBlock, (lps_beacon0y - minY) * zoom - 8);
                anchor1TextBlock.Text = "1";
                mapCanvas.Children.Add(anchor1TextBlock);

                Rectangle anchor2Rect = new Rectangle();
                anchor2Rect.Stroke = Brushes.Yellow;
                anchor2Rect.Margin = new Thickness((lps_beacon1x - 5 - minX) * zoom, (lps_beacon1y - 5 - minY) * zoom, 0, 0);
                anchor2Rect.Width = 10 * zoom;
                anchor2Rect.Height = 10 * zoom;
                mapCanvas.Children.Add(anchor2Rect);

                TextBlock anchor2TextBlock = new TextBlock();
                anchor2TextBlock.Foreground = Brushes.Magenta;
                Canvas.SetLeft(anchor2TextBlock, (lps_beacon1x - minX) * zoom - 3);
                Canvas.SetTop(anchor2TextBlock, (lps_beacon1y - minY) * zoom - 8);
                anchor2TextBlock.Text = "2";
                mapCanvas.Children.Add(anchor2TextBlock);

                Rectangle anchor3Rect = new Rectangle();
                anchor3Rect.Stroke = Brushes.Yellow;
                anchor3Rect.Margin = new Thickness((lps_beacon2x - 5 - minX) * zoom, (lps_beacon2y - 5 - minY) * zoom, 0, 0);
                anchor3Rect.Width = 10 * zoom;
                anchor3Rect.Height = 10 * zoom;
                mapCanvas.Children.Add(anchor3Rect);

                TextBlock anchor3TextBlock = new TextBlock();
                anchor3TextBlock.Foreground = Brushes.Magenta;
                Canvas.SetLeft(anchor3TextBlock, (lps_beacon2x - minX) * zoom - 3);
                Canvas.SetTop(anchor3TextBlock, (lps_beacon2y - minY) * zoom - 8);
                anchor3TextBlock.Text = "3";
                mapCanvas.Children.Add(anchor3TextBlock);

                // Draw path
                if (path != null && path.Length >= 3)
                {
                    int prevX = path[0] * 10;
                    int prevY = path[1] * 10;
                    int len = path[2] & 0x3F;

                    for (int i = 0; i < len; ++i)
                    {
                        int val = ((path[2 + (i * 3 + 6) / 8] & 0xFF) >> ((i * 3 + 6) % 8)) & 7;

                        if (((i * 3 + 6) % 8) > 5)
                            val |= ((path[3 + (i * 3 + 6) / 8] & 0xFF) << (8 - ((i * 3 + 6) % 8))) & 7;

                        if (val == 4)
                            val = 8;

                        string str = IntToString(val, new char[] { '0', '1', '2' });

                        if (str.Length == 1)
                            str = "0" + str;

                        int coord_x = prevX + ((str[0] - '0') - 1) * 10;
                        int coord_y = prevY + ((str[1] - '0') - 1) * 10;

                        Line pathLine = new Line();
                        pathLine.Stroke = Brushes.Magenta;
                        pathLine.X1 = (prevX - minX) * zoom;
                        pathLine.Y1 = (prevY - minY) * zoom;
                        pathLine.X2 = (coord_x - minX) * zoom;
                        pathLine.Y2 = (coord_y - minY) * zoom;
                        pathLine.StrokeThickness = 2;
                        mapCanvas.Children.Add(pathLine);

                        prevX = coord_x;
                        prevY = coord_y;
                    }
                }

                Ellipse carPosEllipse = new Ellipse();
                carPosEllipse.StrokeThickness = 2;
                carPosEllipse.Stroke = Brushes.Lime;
                carPosEllipse.Width = 15 * zoom;
                carPosEllipse.Height = 15 * zoom;
                carPosEllipse.Margin = new Thickness((lps_x - minX) * zoom - 7.5 * zoom, (lps_y - minY) * zoom - 7.5 * zoom, 0, 0);
                mapCanvas.Children.Add(carPosEllipse);

                Line carDirLine = new Line();
                carDirLine.Stroke = Brushes.Green;
                carDirLine.X1 = (lps_x - minX) * zoom;
                carDirLine.Y1 = (lps_y - minY) * zoom;
                carDirLine.X2 = (lps_x + Math.Cos(dir * Math.PI / 180) * 20 - minX) * zoom;
                carDirLine.Y2 = (lps_y + Math.Sin(dir * Math.PI / 180) * 20 - minY) * zoom;
                carDirLine.StrokeThickness = 2;
                mapCanvas.Children.Add(carDirLine);

                Line carFrontDistLine = new Line();
                carFrontDistLine.Stroke = Brushes.Cyan;
                carFrontDistLine.X1 = (lps_x - minX) * zoom;
                carFrontDistLine.Y1 = (lps_y - minY) * zoom;
                carFrontDistLine.X2 = (lps_x + Math.Cos(dir * Math.PI / 180) * frontDist - minX) * zoom;
                carFrontDistLine.Y2 = (lps_y + Math.Sin(dir * Math.PI / 180) * frontDist - minY) * zoom;
                carFrontDistLine.StrokeThickness = 2;
                mapCanvas.Children.Add(carFrontDistLine);

                Line carLeftDistLine = new Line();
                carLeftDistLine.Stroke = Brushes.Cyan;
                carLeftDistLine.X1 = (lps_x - minX) * zoom;
                carLeftDistLine.Y1 = (lps_y - minY) * zoom;
                carLeftDistLine.X2 = (lps_x + Math.Cos((dir - 25) * Math.PI / 180) * leftDist - minX) * zoom;
                carLeftDistLine.Y2 = (lps_y + Math.Sin((dir - 25) * Math.PI / 180) * leftDist - minY) * zoom;
                carLeftDistLine.StrokeThickness = 2;
                mapCanvas.Children.Add(carLeftDistLine);

                Line carRightDistLine = new Line();
                carRightDistLine.Stroke = Brushes.Cyan;
                carRightDistLine.X1 = (lps_x - minX) * zoom;
                carRightDistLine.Y1 = (lps_y - minY) * zoom;
                carRightDistLine.X2 = (lps_x + Math.Cos((dir + 25) * Math.PI / 180) * rightDist - minX) * zoom;
                carRightDistLine.Y2 = (lps_y + Math.Sin((dir + 25) * Math.PI / 180) * rightDist - minY) * zoom;
                carRightDistLine.StrokeThickness = 2;
                mapCanvas.Children.Add(carRightDistLine);

                Line carBackDistLine = new Line();
                carBackDistLine.Stroke = Brushes.Cyan;
                carBackDistLine.X1 = (lps_x - minX) * zoom;
                carBackDistLine.Y1 = (lps_y - minY) * zoom;
                carBackDistLine.X2 = (lps_x - Math.Cos(dir * Math.PI / 180) * backDist - minX) * zoom;
                carBackDistLine.Y2 = (lps_y - Math.Sin(dir * Math.PI / 180) * backDist - minY) * zoom;
                carBackDistLine.StrokeThickness = 2;
                mapCanvas.Children.Add(carBackDistLine);

                if (has_target)
                {
                    Ellipse targetEllipse = new Ellipse();
                    targetEllipse.StrokeThickness = 1;
                    targetEllipse.Stroke = Brushes.LightCyan;
                    targetEllipse.Width = 5 * zoom;
                    targetEllipse.Height = 5 * zoom;
                    targetEllipse.Margin = new Thickness((target_x - minX) * zoom - 2.5 * zoom, (target_y - minY) * zoom - 2.5 * zoom, 0, 0);
                    mapCanvas.Children.Add(targetEllipse);
                }
            }));
        }

        private string IntToString(int value, char[] baseChars)
        {
            string result = string.Empty;
            int targetBase = baseChars.Length;

            do
            {
                result = baseChars[value % targetBase] + result;
                value = value / targetBase;
            }
            while (value > 0);

            return result;
        }

        private void MapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(mapCanvas);
            int minX = Math.Min(lps_beacon0x, Math.Min(lps_beacon1x, Math.Min(lps_beacon2x, lps_x))) - 5;
            int minY = Math.Min(lps_beacon0y, Math.Min(lps_beacon1y, Math.Min(lps_beacon2y, lps_y))) - 5;
            int maxX = Math.Max(lps_beacon0x, Math.Max(lps_beacon1x, Math.Max(lps_beacon2x, lps_x))) + 5;
            int maxY = Math.Max(lps_beacon0y, Math.Max(lps_beacon1y, Math.Max(lps_beacon2y, lps_y))) + 5;
            float zoom = (float)Math.Min(mapCanvas.ActualWidth / (maxX - minX), mapCanvas.ActualHeight / (maxY - minY));
            target_x = (short)(minX + clickPoint.X / zoom);
            target_y = (short)(minY + clickPoint.Y / zoom);
            has_target = true;
            serialPort.WriteLine($"target: {target_x};{target_y}");
            RedrawMap();
        }
    }
}
