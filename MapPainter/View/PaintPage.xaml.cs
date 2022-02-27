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

namespace MapPainter.View
{
    public partial class PaintPage : Page
    {
        private List<double> angles = new();
        private List<double> lengths = new();
        private double startX = 0;
        private double startY = 0;

        public PaintPage(string portName)
        {
            InitializeComponent();
            inkCanvas.AddHandler(MouseDownEvent, new MouseButtonEventHandler(CanvasMouseDown), true);
            startX = Canvas.GetLeft(startPosition);
            startY = 420 - Canvas.GetTop(startPosition);

        }

        private StylusPoint firstPoint;
        private bool flag;

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!flag)
            {
                //Stroke stroke = new (new StylusPointCollection()
                //{
                //    new StylusPoint(startX, 420 - startY),
                //    new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y),
                //});
                //stroke.DrawingAttributes.Color = Colors.AliceBlue;
                //if (e.GetPosition(inkCanvas).Y <= 420 - startY)
                //    angles.Add(90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - startX) / (e.GetPosition(inkCanvas).Y - 420 + startY)));
                //else
                //    angles.Add(-90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - startX) / (e.GetPosition(inkCanvas).Y - 420 + startY)));

                //lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - startX, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - 420 + startY, 2)));

                //inkCanvas.Strokes.Add(stroke);

                firstPoint = new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);
                Canvas.SetLeft(startPosition, e.GetPosition(this).X-startPosition.Width/2);
                Canvas.SetTop(startPosition, e.GetPosition(this).Y-startPosition.Height/2);
                startPosition.Visibility = Visibility.Visible;
                flag = true;
                
            }
            else
            {

                foreach (Stroke s in inkCanvas.Strokes)
                    firstPoint = s.StylusPoints[^1];

                Stroke stroke = new (new StylusPointCollection()
                {
                    firstPoint,
                    new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y),
                });
                stroke.DrawingAttributes.Color = Colors.AliceBlue;
                
                if((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)) != 0)
                {
                    lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)));

                    if (e.GetPosition(inkCanvas).Y <= firstPoint.Y)
                        angles.Add(90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y)));
                    else
                        angles.Add(-90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y)));
                    inkCanvas.Strokes.Add(stroke);
                }
                    
            }
        }

        private string GetRoute()
        {
            string route = string.Empty;
            for (int i = 0; i < lengths.Count; ++i)
                route += $"{angles[i]};{lengths[i]}\n";
            return route;
        }

        private void ButtonClose(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void BorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Application.Current.MainWindow.DragMove();
        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new ConnectPage();
        }

        private void LaunchButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(GetRoute());
        }

        private bool isMoving = false;
        private void startPosition_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMoving = true;

            inkCanvas.Strokes.Clear();
            angles.Clear();
            lengths.Clear();

            startPosition.CaptureMouse();
        }

        private void startPosition_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            startPosition.ReleaseMouseCapture();
            double sX, sY;
            sX = e.GetPosition(inkCanvas).X;
            sY = e.GetPosition(inkCanvas).Y;

            if (e.GetPosition(this).X-startPosition.Width/2 < 0)
                sX = 0;
            else if (e.GetPosition(this).X-startPosition.Width/2 > 1186)
                sX = 1186;

            if (e.GetPosition(this).Y-startPosition.Height/2 < 0)
                sY = 0;
            else if (e.GetPosition(this).Y-startPosition.Height/2 > 555)
                sY = 555;

            firstPoint = new StylusPoint(sX,sY);
        }

        private void startPosition_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                if(e.GetPosition(this).X-startPosition.Width/2 > 0 && e.GetPosition(this).X-startPosition.Width/2 < 1186)
                    Canvas.SetLeft(startPosition, e.GetPosition(this).X-startPosition.Width/2);
                if(e.GetPosition(this).Y-startPosition.Height/2 > 0 && e.GetPosition(this).Y-startPosition.Height/2 < 555)
                    Canvas.SetTop(startPosition, e.GetPosition(this).Y-startPosition.Height/2);
            }
        }
    }
}