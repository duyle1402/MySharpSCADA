using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class HVAC_compressor : HMIControlBase
    {
        static HVAC_compressor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HVAC_compressor), new FrameworkPropertyMetadata(typeof(HVAC_compressor)));
        }
    }
  
}
