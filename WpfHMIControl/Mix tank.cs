using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Mix_tank : HMIControlBase
    {
        static Mix_tank()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Mix_tank), new FrameworkPropertyMetadata(typeof(Mix_tank)));
        }
    }
}
