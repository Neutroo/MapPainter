using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Ports;

namespace MapPainter.View
{
    public partial class PaintPage : Page
    {
        private SerialPort serialPort;
        private List<double> angles = new();
        private List<double> lengths = new();
        private List<StylusPoint> points = new();
        private StylusPoint firstPoint;
        private bool isFirstPointCreated;
        private double first_angle = 0;

        public PaintPage(string portName)
        {
            InitializeComponent();
            inkCanvas.AddHandler(MouseDownEvent, new MouseButtonEventHandler(CanvasMouseDown), true);           
            
            serialPort = new(portName);
            serialPort.BaudRate = 9600;
            if (!serialPort.IsOpen)
                serialPort.Open();
            else
                throw new Exception("port already open");
            
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);                     
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isFirstPointCreated)
            {
                firstPoint = new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);
                
                // Set position for image where we clicked
                Canvas.SetLeft(robot, e.GetPosition(this).X - robot.Width / 2);    
                Canvas.SetTop(robot, e.GetPosition(this).Y - robot.Height / 2);
                
                isFirstPointCreated = true;
                robot.Visibility = Visibility.Visible;
            }
            else
            {
                foreach (Stroke s in inkCanvas.Strokes)
                    firstPoint = s.StylusPoints[^1];

                StylusPoint secondPoint = new(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);
                Stroke stroke = new (new StylusPointCollection()
                {
                    firstPoint,
                    secondPoint,
                });

                points.Add(secondPoint);

                stroke.DrawingAttributes.Color = Colors.AliceBlue;

                if((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)) != 0)
                {
                    if (angles.Count == 0)
                    {
                        lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)));
                        if (e.GetPosition(inkCanvas).Y <= firstPoint.Y)
                            angles.Add((int)(90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y))));
                        else
                            angles.Add((int)(-90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y))));
                        
                        first_angle = angles[0];
                        inkCanvas.Strokes.Add(stroke);
                    }
                    else
                    {
                        lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)));
                        if (e.GetPosition(inkCanvas).Y <= firstPoint.Y)
                            angles.Add((int)(90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y)))-first_angle);
                        else
                            angles.Add((int)(-90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y)))-first_angle);
                        inkCanvas.Strokes.Add(stroke);
                    }
                }                    
            }
        }

        private void ButtonClose(object sender, RoutedEventArgs e)
        { 
            serialPort.Close();
            Application.Current.Shutdown();
        }

        private void BorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Application.Current.MainWindow.DragMove();
        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            serialPort.Close();
            Application.Current.MainWindow.Content = new ConnectPage();
        }

        private void LaunchButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //serialPort.WriteLine(GetRoute());
            }
            catch
            {

            }
            MessageBox.Show(GetRoute());
        }

        private bool isMoving;
        
        private void RobotMouseDown(object sender, MouseButtonEventArgs e)
        {
            isMoving = true;

            inkCanvas.Strokes.Clear();
            angles.Clear();
            lengths.Clear();
            points.Clear();

            robot.CaptureMouse();
        }

        private void RobotMouseUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            
            robot.ReleaseMouseCapture();
            
            double sX, sY;
            sX = e.GetPosition(inkCanvas).X;
            sY = e.GetPosition(inkCanvas).Y;

            if (e.GetPosition(this).X - robot.Width / 2 < 20)
                sX = 0;
            else if (e.GetPosition(this).X - robot.Width / 2 > 1200)
                sX = 1186;

            if (e.GetPosition(this).Y - robot.Height / 2 < 40)
                sY = 0;
            else if (e.GetPosition(this).Y - robot.Height / 2 > 575)
                sY = 555;

            firstPoint = new StylusPoint(sX,sY);
        }

        private void RobotMouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                if(e.GetPosition(this).X - robot.Width / 2 > 20 && e.GetPosition(this).X - robot.Width / 2 < 1200)
                    Canvas.SetLeft(robot, e.GetPosition(this).X - robot.Width / 2);
                if(e.GetPosition(this).Y - robot.Height / 2 > 40 && e.GetPosition(this).Y - robot.Height / 2 < 585)
                    Canvas.SetTop(robot, e.GetPosition(this).Y - robot.Height / 2);
            }
        }

        private string GetRoute()
        {
            string route = string.Empty;
            for (int i = 0; i < lengths.Count; ++i)
                route += $"{angles[i]} {lengths[i]}\n";
            return route;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //SerialPort port = (SerialPort)sender;
            //string line = port.ReadLine();

            //string[] lines = line.Split(' ');

            //if (lines[0] == "r") // If robot sends that he has reached the angle
            //{
            //    Canvas.SetLeft(robot, points[int.Parse(lines[1])].X);
            //    Canvas.SetTop(robot, points[int.Parse(lines[1])].Y);
            //}
        }
    }
}