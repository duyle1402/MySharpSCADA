using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Silo_4 : HMIControlBase
    {
        static Silo_4()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Silo_4), new FrameworkPropertyMetadata(typeof(Silo_4)));
        }
    } 
}
