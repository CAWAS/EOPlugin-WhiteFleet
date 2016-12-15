using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Window;
using ElectronicObserver.Window.Plugins;

namespace WhiteFleet
{
    public class WhiteFleet : ServerPlugin
    {
	    public override string MenuTitle => "ホワイト艦隊";

	    public override bool RunService(FormMain main)
	    {
			Task.Factory.StartNew(() => WhiteObserver.Instance.Initialize());

			return true;
	    }
    }
}
