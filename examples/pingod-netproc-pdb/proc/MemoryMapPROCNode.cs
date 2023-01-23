using PinGod.Core;
using PinGod.Core.Service;
using System;

public partial class MemoryMapPROCNode : MemoryMapNode
{
    PinGodGameProc _pinGodProc;

    /// <summary>
    /// This is where the base memory map is created. Need to have access to PROC to create with game controller.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        _pinGodProc = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
    }

    public override void CreateMemoryMap()
    {
        //todo: get proc gamecontroller
        if(_pinGodProc?.PinGodProcGame != null)
        {            
            mMap = new MemoryMapPROC(_pinGodProc.PinGodProcGame, this.MutexName, MapName, WriteDelay, ReadDelay, CoilTotal, LampTotal, LedTotal, SwitchTotal);            
        }
        else
        {
            Logger.WarningRich(nameof(MemoryMapPROC), ": [color=yellow]No PinGodProcGame found, exiting memory mapping[/color]");
            this.QueueFree();
        }
    }

    internal void WriteStates()
    {
        (mMap as MemoryMapPROC).WriteProcStates();
    }
}
