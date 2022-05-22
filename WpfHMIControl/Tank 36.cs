using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Tank_36 : HMIControlBase
    {
        static Tank_36()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tank_36), new FrameworkPropertyMetadata(typeof(Tank_36)));
        }
    }
 }
