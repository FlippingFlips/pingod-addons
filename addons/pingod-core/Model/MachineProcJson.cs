using PinGod.Base;
using PinGod.Core;
using PinGodAddOns.addons.pingod_core.Model;
using System.Collections.Generic;

public class MachineProcJson
{
    public MachineProcBallSearch PRBallSearch { get; set; }
    public IEnumerable<PinStateObject> PRCoils { get; set; }
    public IEnumerable<PinStateObject> PRLamps { get; set; }
    public IEnumerable<PinStateObject> PRLeds { get; set; }        
    public IEnumerable<Switch> PRSwitches { get; set; }
}
