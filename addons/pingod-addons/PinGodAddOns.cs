#if TOOLS
using Godot;

/// <summary>
/// This add custom types to use in the Godot editor
/// </summary>
[Tool]
public partial class PinGodAddOns : EditorPlugin
{
	const string ROOT_DIR = "addons/pingod-addons/";

    /// <summary>
    /// Initialization of the PlugIn. Adds CustomTypes new type with a name, a parent type, a script and an icon.
    /// </summary>
    public override void _EnterTree()
	{
        using var texture = GD.Load<Texture2D>("res://addons/assets/img/pinball.png");

        var script = GD.Load<Script>(ROOT_DIR+"Labels/BlinkingLabel.cs");
		AddCustomType("PinGod" + nameof(BlinkingLabel), nameof(Label), script, texture);

		script = GD.Load<Script>(ROOT_DIR + "VideoPlayers/VideoPlayerPinball.cs");
		AddCustomType("PinGod" + nameof(VideoPlayerPinball), nameof(VideoStreamPlayer), script, texture);

		script = GD.Load<Script>(ROOT_DIR + "Timers/BallStackPinball.cs");
		AddCustomType("PinGod" + nameof(BallStackPinball), nameof(Timer), script, texture);		

		script = GD.Load<Script>(ROOT_DIR + "Lanes/PinballLanesNode.cs");
		AddCustomType("PinGod" + nameof(PinballLanesNode), nameof(Node), script, texture);

		script = GD.Load<Script>(ROOT_DIR + "Targets/PinballTargetsBank.cs");
		AddCustomType("PinGod" + nameof(PinballTargetsBank), nameof(Node), script, texture);
	}

	/// <summary>
	/// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
	/// </summary>
	public override void _ExitTree()
	{
		RemoveCustomType(nameof(BlinkingLabel));
		RemoveCustomType(nameof(BallStackPinball));		
		RemoveCustomType(nameof(PinballLanesNode));
		RemoveCustomType(nameof(PinballTargetsBank));
		RemoveCustomType(nameof(VideoPlayerPinball));
	}
}
#endif
