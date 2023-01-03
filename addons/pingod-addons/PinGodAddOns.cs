#if TOOLS
using Godot;

/// <summary>
/// This add custom types to use in the Godot editor
/// </summary>
[Tool]
public partial class PinGodAddOns : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "addons/pingod-addons/";

    /// <summary>
    /// Initialization of the PlugIn. Adds CustomTypes new type with a name, a parent type, a script and an icon.
    /// </summary>
    public override void _EnterTree()
	{
        Logger.Debug(nameof(PinGodAddOns), nameof(_EnterTree), ":" + ROOT_DIR);

        using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");

        var script = GD.Load<Script>(ROOT_DIR+$"Labels/{nameof(BlinkingLabel)}.cs");
		AddCustomType("PinGod" + nameof(BlinkingLabel), nameof(Label), script, texture);

        script = GD.Load<Script>(ROOT_DIR + $"Labels/{nameof(CreditsLabel)}.cs");
        AddCustomType("PinGod" + nameof(CreditsLabel), nameof(Label), script, texture);

        script = GD.Load<Script>(ROOT_DIR + $"VideoPlayers/{nameof(VideoPlayerPinball)}.cs");
		AddCustomType("PinGod" + nameof(VideoPlayerPinball), nameof(VideoStreamPlayer), script, texture);

		script = GD.Load<Script>(ROOT_DIR + $"Timers/{nameof(Saucer)}.cs");
		AddCustomType("PinGod" + nameof(Saucer), nameof(Timer), script, texture);		

		script = GD.Load<Script>(ROOT_DIR + $"Lanes/{nameof(PinballLanesNode)}.cs");
		AddCustomType("PinGod" + nameof(PinballLanesNode), nameof(Node), script, texture);

        Logger.Debug(nameof(PinGodAddOns), ":" + nameof(_EnterTree), " loaded custom types");
    }

	/// <summary>
	/// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
	/// </summary>
	public override void _ExitTree()
	{
        Logger.Debug(nameof(PinGodAddOns), ":" + nameof(_EnterTree), " removing custom types");
        RemoveCustomType(nameof(BlinkingLabel));
		RemoveCustomType(nameof(Saucer));		
		RemoveCustomType(nameof(PinballLanesNode));
		RemoveCustomType(nameof(TargetsBank));
		RemoveCustomType(nameof(VideoPlayerPinball));
	}
}
#endif
