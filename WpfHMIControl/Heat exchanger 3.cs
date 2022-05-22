using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Heat_exchanger_3 : HMIControlBase
    {
        static Heat_exchanger_3()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Heat_exchanger_3), new FrameworkPropertyMetadata(typeof(Heat_exchanger_3)));
        }
    }
   
}
