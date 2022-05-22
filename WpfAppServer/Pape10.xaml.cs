using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Linq;
using BuildingDemo;
using System.Windows.Input;

namespace WpfAppServer
{
    public partial class Pape10 : UserControl
    {
        List<TagNodeHandle> _valueChangedList;

        // test
        private BuildingDemo.ViewModel viewModel;


        public Pape10()
        {
            InitializeComponent();

            // test
            this.DataContext = this.viewModel = new ViewModel();

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
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = (HelixViewport3D)sender;
            var firstHit = viewport.Viewport.FindHits(e.GetPosition(viewport)).FirstOrDefault();
            if (firstHit != null)
            {
                this.viewModel.Select(firstHit.Visual);
            }
            else
            {
                this.viewModel.Select(null);
            }
        }

    }
}
