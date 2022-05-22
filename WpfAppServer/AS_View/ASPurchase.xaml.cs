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

namespace WpfAppServer.AS_View
{
    /// <summary>
    /// Interaction logic for ASPurchase.xaml
    /// </summary>
    public partial class ASPurchase : Window
    {
        public ASPurchase()
        {
            InitializeComponent();
            this.SetWindowState();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            this.SaveWindowState();
        }
    }
}
