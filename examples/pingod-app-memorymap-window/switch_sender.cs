using Godot;

public partial class switch_sender : Node
{
    private PinGodMemoryMapNode _memMap;
    private PinGodMachine _machine;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (HasNode("/root/MemoryMap"))
		{
			_memMap = GetNodeOrNull<PinGodMemoryMapNode>("/root/MemoryMap");			
		}

        if (HasNode("/root/Machine"))
        {
            _machine = GetNodeOrNull<PinGodMachine>("/root/Machine");
			Logger.Log(LogLevel.Info, Logger.BBColor.green, "switch count:", Machine.Switches.Count);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	byte state = 1;
	public void _on_button_button_up()
	{		
        //send coin
        _memMap?.WriteSwitchPulseToMemory(3, state);
		if (state > 0) state = 0;
		else state++;
    }
}
