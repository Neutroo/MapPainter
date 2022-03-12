using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapPainter.View
{
    public partial class ConnectPage : Page
    {
        public ConnectPage()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();

            if (ports != null)
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
            => Application.Current.MainWindow.Content = new PaintPage(comboBoxPorts.Text);
    }
}