using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Clarifier : HMIControlBase
    {
        static Clarifier()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Clarifier), new FrameworkPropertyMetadata(typeof(Clarifier)));
        }
    }
    
}
