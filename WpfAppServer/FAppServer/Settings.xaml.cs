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
using System.Windows.Shapes;

namespace WpfAppServer.FAppServer
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            this.SetWindowState();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.SaveWindowState();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
