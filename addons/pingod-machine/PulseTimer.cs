using Godot;
using PinGod.Base;

/// <summary>This pulse timer is a godot timer which will set the coil state to 0 when complete</summary>
public partial class PulseTimer : Timer
{
    public override void _Ready()
    {
        base._Ready();

        if(string.IsNullOrWhiteSpace(Name))
        {
            this.QueueFree();
            return;
        }

        Logger.Verbose(nameof(PulseTimer), ":", nameof(_Ready), ":", Name);
        WaitTime = WaitTime / 1000;

        if (Machine.Coils.ContainsKey(Name)) Machine.Coils[Name].State = 1;
        else
        {
            Logger.Log(LogLevel.Error, Logger.BBColor.red,
                nameof(_EnterTree), $":Pulse timer -- No coil found for {Name}");
            Name = string.Empty;
            QueueFree();
            return;
        }

        this.Connect("timeout", new Callable(this, nameof(PulseTimer_Timeout)));
        Start();
    }

    private void PulseTimer_Timeout() => this.QueueFree();

    public override void _ExitTree()
    {
        Machine.Coils[Name].State = 0;
        base._ExitTree();        
    }
}
