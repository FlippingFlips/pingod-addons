using Godot;
using System.Linq;

/// <summary>
/// SwitchOverlay GridContainer. Creates buttons from Machine.Switches and connects to button events to fire switches off
/// </summary>
public partial class SwitchOverlayMemMap : SwitchOverlay
{
	private Switches _switches;
    private PinGodMemoryMapNode _memMap;

    /// <summary>
    /// 
    /// </summary>
    public override void _EnterTree()
	{
		if (!Engine.IsEditorHint())
		{
            Logger.LogLevel = LogLevel.Verbose;
            _switches = Machine.Switches;//?.Values.OrderBy(x => x.Num);

            foreach (var sw in _switches.Values.OrderBy(x=>x.Num))
            {
                var button = new Button() { Text = sw.Name, ToggleMode = true };
                button.CustomMinimumSize = new Vector2(100, 50);
                AddChild(button);
                button.Toggled += ((pressed) => OnToggle(pressed, sw.Name));
                //button.Connect("toggled", new Callable(this, nameof(OnToggle)));
            }
        }
	}

    public override void _Ready()
    {
        base._Ready();
        if(HasNode("/root/MemoryMap"))
        {
            _memMap = GetNode<PinGodMemoryMapNode>("/root/MemoryMap");            
        }
    }

    protected override void OnToggle(bool button_pressed, string swName)
    {
        //base.OnToggle(button_pressed, swName);
		Logger.Log(LogLevel.Info, Logger.BBColor.red, $"button:", swName, " was pressed ", button_pressed);
        _memMap?.WriteSwitchToMemory(Machine.Switches[swName].Num, (byte)(button_pressed ? 1 : 0));
    }
}
