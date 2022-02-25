using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapPainter.View
{ 
    public partial class ConnectPage : Page
    {
        public ConnectPage()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();
            
            foreach (string port in ports)
                comboBoxPorts.Items.Add(port);

            comboBoxPorts.SelectedIndex = 0;
        }

        private void ButtonClose(object sender, RoutedEventArgs e) 
            => Application.Current.Shutdown();

        private void BorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Application.Current.MainWindow.DragMove();
        }

        private void ButtonConnect(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new PaintPage(comboBoxPorts.Text);
        }
    }
}