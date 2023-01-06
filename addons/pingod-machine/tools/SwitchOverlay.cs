using Godot;

/// <summary>
/// SwitchOverlay GridContainer. Creates buttons from Machine.Switches and connects to button events to fire switches off
/// </summary>
public partial class SwitchOverlay : GridContainer
{
	private Switches _switches;

	/// <summary>
	/// 
	/// </summary>
	public override void _EnterTree()
	{
		_switches = Machine.Switches;
		foreach (var sw in _switches)
		{
			var button = new Button() { Text = $"{sw.Value.Num}-{sw.Key}", ToggleMode = true };
			AddChild(button);
            button.Toggled += ((pressed) => OnToggle(pressed, sw.Key));			
			//button.Connect("toggled", new Callable(this, nameof(OnToggle)));
		}
	}

	/// <summary>
	/// Runs SetSwitch on the Machine.Switches
	/// </summary>
	/// <param name="button_pressed"></param>
	/// <param name="swName"></param>
    protected virtual void OnToggle(bool button_pressed, string swName)
	{
		Logger.Verbose("switch overlay: " + swName + button_pressed);
		var sw = Machine.Switches[swName];
		sw?.SetSwitch((byte)(button_pressed ? 1 : 0));
	}
}
