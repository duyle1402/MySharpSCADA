using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class Industrial_platform_1 : HMIControlBase
    {
        static Industrial_platform_1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Industrial_platform_1), new FrameworkPropertyMetadata(typeof(Industrial_platform_1)));
        }
    }
  
}
