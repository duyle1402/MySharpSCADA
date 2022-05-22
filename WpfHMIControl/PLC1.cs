using HMIControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfHMIControl
{
    public class PLC1 : HMIControlBase
    {
        static PLC1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PLC1), new FrameworkPropertyMetadata(typeof(PLC1)));
        }

        public override string[] GetActions()
        {
            return new string[] { TagActions.VISIBLE, TagActions.CAPTION, TagActions.RUN, "RUN_LOCAL", TagActions.ALARM, TagActions.DEVICENAME };
        }

        public override LinkPosition[] GetLinkPositions()
        {
            return new LinkPosition[4]
                {
                    new  LinkPosition(new Point(0,0.5),ConnectOrientation.Left),
                    new  LinkPosition(new Point(1,0.5),ConnectOrientation.Right),
                    new  LinkPosition(new Point(0.5,0),ConnectOrientation.Top),
                    new  LinkPosition(new Point(0.5,1),ConnectOrientation.Bottom),
                };
        }

        public override Action SetTagReader(string key, Delegate tagChanged)
        {
            switch (key)
            {
                case TagActions.RUN:
                    var _funcInRun = tagChanged as Func<bool>;
                    if (_funcInRun != null)
                    {
                        return delegate { VisualStateManager.GoToState(this, _funcInRun() ? "Running" : "NotRunning", true); };
                    }
                    else return null;
            }
            return base.SetTagReader(key, tagChanged);
        }
    }
}
