using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Diaphragm_pump_2 : HMIControlBase
    {
        static Diaphragm_pump_2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Diaphragm_pump_2), new FrameworkPropertyMetadata(typeof(Diaphragm_pump_2)));
        }
    }
}
