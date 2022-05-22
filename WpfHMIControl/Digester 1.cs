using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Digester_1 : HMIControlBase
    {
        static Digester_1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Digester_1), new FrameworkPropertyMetadata(typeof(Digester_1)));
        }
    }
}
