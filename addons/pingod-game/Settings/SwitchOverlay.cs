using Godot;
using System.Xml.Linq;

/// <summary>
/// SwitchOverlay GridContainer. Creates buttons from Machine.Switches and connects to button events to fire switches off
/// </summary>
public partial class SwitchOverlay : GridContainer
{
	private Switches _switches;

	PinGodGame pingod;

	/// <summary>
	/// 
	/// </summary>
	public override void _EnterTree()
	{
		_switches = Machine.Switches;
		foreach (var sw in _switches)
		{
			var button = new Button() { Text = sw.Key, ToggleMode = true };
			AddChild(button);
            button.Toggled += ((pressed) => OnToggle(pressed, sw.Key));			
			//button.Connect("toggled", new Callable(this, nameof(OnToggle)));
		}

		pingod = GetNode("/root/PinGodGame") as PinGodGame;
	}

    void OnToggle(bool button_pressed, string swName)
	{
		Logger.Verbose("switch overlay: " + swName + button_pressed);
		var sw = Machine.Switches[swName];
		sw?.SetSwitch(button_pressed);
	}
}
