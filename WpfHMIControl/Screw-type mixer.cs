using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Screw_type_mixer : HMIControlBase
    {
        static Screw_type_mixer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Screw_type_mixer), new FrameworkPropertyMetadata(typeof(Screw_type_mixer)));
        }
    }  
}
