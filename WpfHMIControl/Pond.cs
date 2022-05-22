using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Pond : HMIControlBase
    {
        static Pond()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pond), new FrameworkPropertyMetadata(typeof(Pond)));
        }
    }
}
