using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Exposed_ladder : HMIControlBase
    {
        static Exposed_ladder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Exposed_ladder), new FrameworkPropertyMetadata(typeof(Exposed_ladder)));
        }
    }
   
}
