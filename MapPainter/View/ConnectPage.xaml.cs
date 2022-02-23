using System;
using System.Collections.Generic;
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
            comboBoxPorts.Items.Add("some item1");
            comboBoxPorts.Items.Add("some item2");
            comboBoxPorts.Items.Add("some item3");
            comboBoxPorts.SelectedIndex = 0;
        }

        private void ButtonClose(object sender, RoutedEventArgs e) 
            => Application.Current.Shutdown();

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Application.Current.MainWindow.DragMove();
        }

        private void ButtonConnect(object sender, RoutedEventArgs e)
        {

        }
    }
}