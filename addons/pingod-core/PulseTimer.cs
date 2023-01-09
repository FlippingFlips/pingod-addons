using Godot;
using PinGod.Base;
using PinGod.Core;

public partial class PulseTimer : Timer
{
    public override void _EnterTree()
    {
        Logger.Verbose(nameof(PulseTimer),":", nameof(_EnterTree), ":", Name);
        WaitTime = WaitTime / 1000;
        base._EnterTree();
        if(Machine.Coils.ContainsKey(Name)) Machine.Coils[Name].State = 1;
        else 
        { 
            GD.PrintRich($"[color=red]Pulse timer -- No coil found for {Name}[/color]");
            Name = string.Empty;
            QueueFree();
            return;
        }      
    }

    public override void _Ready()
    {
        base._Ready();
        this.Connect("timeout", new Callable(this, nameof(PulseTimer_Timeout)));
        Start();
    }

    private void PulseTimer_Timeout()
    {
        Machine.Coils[Name].State = 0;
        this.QueueFree();
    }
}
