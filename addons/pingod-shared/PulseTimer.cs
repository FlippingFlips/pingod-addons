using Godot;

public partial class PulseTimer : Timer
{
    public override void _EnterTree()
    {
        GD.Print(nameof(PulseTimer),":", Name);
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
        GD.Print("pulse time out " + Name);
        this.QueueFree();
    }
}
