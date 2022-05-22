using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Particles : HMIControlBase
    {
        static Particles()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Particles), new FrameworkPropertyMetadata(typeof(Particles)));
        }
    }
    
}
