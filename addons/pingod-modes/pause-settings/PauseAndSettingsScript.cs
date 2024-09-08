using Godot;
using PinGod.Core;
using PinGod.Core.Service;

/// <summary>
/// Handles actions pause and quit. This is usually in the main scene but be used on it's own <para/>
/// TODO: this should really be parent with as many controls. Then this could be an overlay to the MainScene for this kind of thing. Maybe even move the service menu here
/// </summary>
public partial class PauseAndSettingsScript : CanvasLayer
{
	private AdjustmentsNode _adjustments;
	private Control pauseLayer;
	private Control settingsDisplay;
	private bool _isPaused;

	public override void _Ready()
	{
		//load Resources node from PinGodGame
		if (HasNode("/root/Adjustments"))
		{
			_adjustments = GetNode("/root/Adjustments") as AdjustmentsNode;
		}
		else Logger.WarningRich(nameof(PauseAndSettingsScript), ":[color=yellow]", "AdjustmentsScript wasn't found in /root/Adjustments. Used for viewing settings screen[/color]");

		//show a pause menu when pause enabled.
		if (this.HasNode("PauseControl"))
			pauseLayer = GetNode<Control>("PauseControl");
		
		//settings display
		if (this.HasNode("SettingsDisplay"))
			settingsDisplay = GetNodeOrNull<Control>("SettingsDisplay");
	}

	/// <summary>
	/// Process input to show pause and settings screens
	/// </summary>
	/// <param name="event"></param>
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (!@event.IsActionType()) return;
		if (@event is InputEventMouse) return;

		if(pauseLayer != null)
		{
			if (InputMap.HasAction("pause") && @event.IsActionPressed("pause"))
			{
				Logger.Verbose(nameof(PauseAndSettingsScript), "pause requested");
				if (settingsDisplay?.Visible ?? false) return;
				SetPaused(!_isPaused);
				if (_isPaused) pauseLayer.Visible = true;
			}
		}        

		if(settingsDisplay != null && _adjustments != null)
		{
			if (InputMap.HasAction("settings") && @event.IsActionPressed("settings"))
			{
                Logger.Verbose(nameof(PauseAndSettingsScript), "settings requested");
                if (pauseLayer?.Visible ?? false) return;                
				SetPaused(!_isPaused);
				if (_isPaused) settingsDisplay.Visible = true;
			}
		}        
	}

	public virtual void SetPaused(bool paused)
	{
		if (!paused)
		{
			if (settingsDisplay != null) settingsDisplay.Visible = false;
			if (pauseLayer != null) pauseLayer.Visible = false;            
		}

		GetNode("/root").GetTree().Paused = paused;
		Logger.Debug(nameof(PauseAndSettingsScript), ": game paused=" + paused);

		_isPaused = paused;
	}
}
