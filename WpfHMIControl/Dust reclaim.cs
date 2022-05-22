using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Dust_reclaim : HMIControlBase
    {
        static Dust_reclaim()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Dust_reclaim), new FrameworkPropertyMetadata(typeof(Dust_reclaim)));
        }

    }
}
