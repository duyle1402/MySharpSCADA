using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Railroad_track : HMIControlBase
    {
        static Railroad_track()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Railroad_track), new FrameworkPropertyMetadata(typeof(Railroad_track)));
        }
    }
   
}
