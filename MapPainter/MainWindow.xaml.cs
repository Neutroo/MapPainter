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
        List<Vector2> vectors = new();

        public MainWindow()
        {
            InitializeComponent();
            inkCanvas.AddHandler(MouseDownEvent, new MouseButtonEventHandler(CanvasMouseDown), true);
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
            MessageBox.Show($"points:{cnt}\nstrokes:{inkCanvas.Strokes.Count}\nvectors:{vectors.Count}");
        }

        private void ButtonEdit(object sender, RoutedEventArgs e)
        {
            coordinates.Text = GetRoute(vectors);
        }

        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            list.Items.Clear();
            vectors.Clear();
            coordinates.Text = string.Empty;
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!inkCanvas.Strokes.Count.Equals(0))
            {
                StylusPoint firstPoint;
                foreach (Stroke s in inkCanvas.Strokes)
                    firstPoint = s.StylusPoints[^1];

                Stroke stroke = new Stroke(new StylusPointCollection()
                {
                    firstPoint,
                    new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y),
                });

                vectors.Add(new Vector2((float)(stroke.GetGeometry().Bounds.TopRight.X - stroke.GetGeometry().Bounds.TopLeft.X), 
                    (float)(stroke.GetGeometry().Bounds.TopRight.Y - stroke.GetGeometry().Bounds.TopLeft.Y)));
                inkCanvas.Strokes.Add(stroke);
            }
        }

        private static string GetRoute(List<Vector2> vectors)
        {       
            List<float> angles = new() { 0 };
            for (int i = 0; i < vectors.Count - 1; ++i)
            {
                //angles.Add(Math.Acos(Math.Abs(geometries[i].Bounds.TopLeft.X * geometries[i + 1].Bounds.TopLeft.X
                //    + geometries[i].Bounds.TopLeft.Y * geometries[i + 1].Bounds.TopLeft.Y)
                //    / (Math.Sqrt(Math.Pow(geometries[i].Bounds.TopLeft.X, 2) + Math.Pow(geometries[i].Bounds.TopLeft.Y, 2))
                //    * Math.Sqrt(Math.Pow(geometries[i + 1].Bounds.TopLeft.X, 2) + Math.Pow(geometries[i + 1].Bounds.TopLeft.Y, 2)))));

                angles.Add((float)Math.Cos(Vector2.Dot(vectors[i], vectors[i + 1]) / (vectors[i].Length() * vectors[i + 1].Length())));
            }

            List<float> lenght = new();
            foreach (Vector2 vec in vectors)
                //lenght.Add((int)Math.Sqrt(Math.Pow(geom.Bounds.TopRight.X - geom.Bounds.TopLeft.X, 2) + Math.Pow(geom.Bounds.TopRight.Y - geom.Bounds.TopLeft.Y, 2)));
                lenght.Add(vec.Length());

            string route = string.Empty;
            for (int i = 0; i < lenght.Count; ++i)
                route += $"{angles[i]};{lenght[i]}\n";
            return route;
        }
    }
}