using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Exhaust_stack : HMIControlBase
    {
        static Exhaust_stack()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Exhaust_stack), new FrameworkPropertyMetadata(typeof(Exhaust_stack)));
        }
    }
 
}
