using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

namespace MapPainter
{
    public partial class MainWindow : Window
    {
        private List<StylusPoint> points = new();
        private List<double> angles = new();
        private List<double> lengths = new();
        private double startX = 0;
        private double startY = 0;


        public MainWindow()
        {
            InitializeComponent();
            inkCanvas.AddHandler(MouseDownEvent, new MouseButtonEventHandler(CanvasMouseDown), true);
            
            Stroke stroke = new Stroke(new StylusPointCollection()
                {
                    new StylusPoint(startX,420-startY),
                    new StylusPoint(startX,420-startY),
                });

            //geometries.Add(new Vector2((float)(stroke.GetGeometry().Bounds.TopRight.X - stroke.GetGeometry().Bounds.BottomLeft.X),
            //    (float)(stroke.GetGeometry().Bounds.TopRight.Y - stroke.GetGeometry().Bounds.BottomLeft.Y)));\
            inkCanvas.Strokes.Add(stroke);
        }

        private void ButtonGet(object sender, RoutedEventArgs e)
        {
            //foreach (var str in strokes)
            //{
            //    MessageBox.Show(str.StylusPoints[str.StylusPoints.Count].ToString());
            //    StylusPoint point = new(str.StylusPoints[str.StylusPoints.Count].X, str.StylusPoints[str.StylusPoints.Count].Y);
            //    str.StylusPoints.Add(point);
            //}

            //foreach (Stroke s in ink.Strokes)
            //    for (int i = 0; i < s.StylusPoints.Count - 1; ++i)
            //        s.StylusPoints.Add(new StylusPoint(100, 100));
            int cnt = 0;
            foreach (Stroke s in inkCanvas.Strokes)
            {
                /*MessageBox.Show($"x:{s.GetGeometry().Bounds.TopLeft.X}, y:{s.GetGeometry().Bounds.TopLeft.Y}\n" +
                    $"x:{s.GetGeometry().Bounds.TopRight.X}, y:{s.GetGeometry().Bounds.TopRight.Y}");*/
                foreach (StylusPoint sp in s.StylusPoints)
                {
                    ++cnt;
                    //coordinates.Text = $"x:{sp.X}, y:{sp.Y}";
                }
            }
            MessageBox.Show($"points:{cnt}\nstrokes:{inkCanvas.Strokes.Count}\n");
        }

        private void ButtonEdit(object sender, RoutedEventArgs e)
        {
            coordinates.Text = GetRoute();
        }

        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            list.Items.Clear();
            points.Clear();
            angles.Clear();
            lengths.Clear();
            coordinates.Text = string.Empty;
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (inkCanvas.Strokes.Count.Equals(0))
            {
                
                Stroke stroke = new Stroke(new StylusPointCollection()
                {
                    new StylusPoint(startX, 420-startY),
                    new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y),
                });

                if (e.GetPosition(inkCanvas).Y <= 420-startY)
                    angles.Add(90+180/3.14159264*Math.Atan((e.GetPosition(inkCanvas).X-startX)/(e.GetPosition(inkCanvas).Y-420+startY)));
                else
                    angles.Add(-90+180/3.14159264*Math.Atan((e.GetPosition(inkCanvas).X-startX)/(e.GetPosition(inkCanvas).Y-420+startY)));
                    
                lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - startX, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - 420+ startY, 2)));

                inkCanvas.Strokes.Add(stroke);
            }
            else
            {
                StylusPoint firstPoint;

                foreach (Stroke s in inkCanvas.Strokes)
                    firstPoint = s.StylusPoints[^1];

                Stroke stroke = new Stroke(new StylusPointCollection()
                {
                    firstPoint,
                    new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y),
                });

                //angles.Add(180/3.14159264*Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X)/(e.GetPosition(inkCanvas).Y - firstPoint.Y)));

                if (e.GetPosition(inkCanvas).Y <= firstPoint.Y)
                    angles.Add(90+180/3.14159264*Math.Atan((e.GetPosition(inkCanvas).X-firstPoint.X)/(e.GetPosition(inkCanvas).Y-firstPoint.Y)));
                else
                    angles.Add(-90+180/3.14159264*Math.Atan((e.GetPosition(inkCanvas).X-firstPoint.X)/(e.GetPosition(inkCanvas).Y-firstPoint.Y)));

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
            cords.Content = $"{startX} ; {startY}";
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            startY = Math.Floor(e.NewValue);
            cords.Content = $"{startX} ; {startY}";
        }
    }
}