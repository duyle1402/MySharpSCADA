using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfAppServer
{
    public partial class Pape7 : UserControl
    {
        List<TagNodeHandle> _valueChangedList;

        public Pape7()
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
