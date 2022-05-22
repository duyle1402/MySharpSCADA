using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Horizontal_railing : HMIControlBase
    {
        static Horizontal_railing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Horizontal_railing), new FrameworkPropertyMetadata(typeof(Horizontal_railing)));
        }
    }
   
}
