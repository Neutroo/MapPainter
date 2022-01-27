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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<StylusPoint> points = new();
            foreach (Stroke s in ink.Strokes)
                foreach (StylusPoint sp in s.StylusPoints)
                {
                    points.Add(sp);
                    coordinates.Text = $"x:{sp.X}, y:{sp.Y}";
                }
            MessageBox.Show($"{points.Count}\n{ink.Strokes.Count}");    
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ink.Strokes.Clear();           
        }
    }
}