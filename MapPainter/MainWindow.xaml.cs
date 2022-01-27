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

namespace MapPainter
{
    public partial class MainWindow : Window
    {
        private List<StylusPoint> points = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonGet(object sender, RoutedEventArgs e)
        {
            var strokes = ink.Strokes;
            //foreach (var str in strokes)
            //{
            //    MessageBox.Show(str.StylusPoints[str.StylusPoints.Count].ToString());
            //    StylusPoint point = new(str.StylusPoints[str.StylusPoints.Count].X, str.StylusPoints[str.StylusPoints.Count].Y);
            //    str.StylusPoints.Add(point);
            //}

            //foreach (Stroke s in ink.Strokes)
            //    for (int i = 0; i < s.StylusPoints.Count - 1; ++i)
            //        s.StylusPoints.Add(new StylusPoint(100, 100));

            foreach (Stroke s in ink.Strokes)
            {
                MessageBox.Show($"x:{s.GetGeometry().Bounds.TopLeft.X}, y:{s.GetGeometry().Bounds.TopLeft.Y}\n" +
                    $"x:{s.GetGeometry().Bounds.TopRight.X}, y:{s.GetGeometry().Bounds.TopRight.Y}");
                //foreach (StylusPoint sp in s.StylusPoints)
                //{
                //    points.Add(sp);
                //    coordinates.Text = $"x:{sp.X}, y:{sp.Y}";
                //}
            }

            //MessageBox.Show($"{points.Count}\n{ink.Strokes.Count}");
        }

        private void ButtonEdit(object sender, RoutedEventArgs e)
        {
            Method();
            //ink.Strokes.Clear();

            foreach (Stroke s in ink.Strokes)              
                foreach (StylusPoint sp in points)
                    s.StylusPoints.Add(sp);
            //foreach (Stroke s in ink.Strokes)
            //    foreach (StylusPoint sp in s.StylusPoints)  
            foreach (StylusPoint point in points)
                list.Items.Add($"x:{point.X}, y:{point.Y}");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ink.Strokes.Clear();
            list.Items.Clear();
        }

        private void Method()
        {
            int coef = 1;

            for (int j = 5; j < 10; j++)     
                if (points.Count % j == 0)              
                    coef = j;                          

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i; j < coef; j++)            
                    points[i + j] = new StylusPoint(0, 0);
                
                i += coef + 1;
            }
        }
    }
}