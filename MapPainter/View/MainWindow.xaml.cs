using System.Windows;

namespace MapPainter.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new ConnectPage();
        }

        private void ButtonClose(object sender, RoutedEventArgs e) 
            => Application.Current.Shutdown();       
    }
}