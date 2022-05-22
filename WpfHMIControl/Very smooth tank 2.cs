using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Very_smooth_tank_2 : HMIControlBase
    {
        static Very_smooth_tank_2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Very_smooth_tank_2), new FrameworkPropertyMetadata(typeof(Very_smooth_tank_2)));
        }
    }

}
