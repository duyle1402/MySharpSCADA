using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfAppServer
{
    public partial class Pape11 : UserControl
    {
        List<TagNodeHandle> _valueChangedList;

        public Pape11()
        {
            InitializeComponent();
        }

        private void HMI_Loaded(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                _valueChangedList = cvs1.BindingToServer(App.Server);
            }
        }

        private void HMI_Unloaded(object sender, RoutedEventArgs e)
        {
            lock (this)
            {
                App.Server.RemoveHandles(_valueChangedList);
            }
        }
    }
}