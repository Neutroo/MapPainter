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

            Stroke stroke = new (new StylusPointCollection()
            {
                new StylusPoint(startX, 420 - startY),
                new StylusPoint(startX, 420 - startY),
            });

            stroke.DrawingAttributes.Color = Color.FromRgb(106, 106, 204);

            inkCanvas.Strokes.Add(stroke);
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (inkCanvas.Strokes.Count.Equals(0))
            {

                Stroke stroke = new (new StylusPointCollection()
                {
                    new StylusPoint(startX, 420 - startY),
                    new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y),
                });

                if (e.GetPosition(inkCanvas).Y <= 420 - startY)
                    angles.Add(90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - startX) / (e.GetPosition(inkCanvas).Y - 420 + startY)));
                else
                    angles.Add(-90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - startX) / (e.GetPosition(inkCanvas).Y - 420 + startY)));

                lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - startX, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - 420 + startY, 2)));

                inkCanvas.Strokes.Add(stroke);
            }
            else
            {
                StylusPoint firstPoint;

                foreach (Stroke s in inkCanvas.Strokes)
                    firstPoint = s.StylusPoints[^1];

                Stroke stroke = new (new StylusPointCollection()
                {
                    firstPoint,
                    new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y),
                });
                stroke.DrawingAttributes.Color = Colors.AliceBlue; //Color.FromRgb(106, 106, 204);

                if (e.GetPosition(inkCanvas).Y <= firstPoint.Y)
                    angles.Add(90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y)));
                else
                    angles.Add(-90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y)));

                lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)));

                inkCanvas.Strokes.Add(stroke);
            }
        }

        private string GetRoute()
        {
            string route = string.Empty;
            for (int i = 0; i < lengths.Count; ++i)
                route += $"{angles[i]};{lengths[i]}\n";
            return route;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            startX = Math.Floor(e.NewValue);
            //cords.Content = $"{startX} ; {startY}";
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            startY = Math.Floor(e.NewValue);
            //cords.Content = $"{startX} ; {startY}";
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
    }
}