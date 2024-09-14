#if TOOLS
using Godot;
using PinGod.Core.addons.Video;
using PinGod.EditorPlugins;

[Tool]
public partial class PinGodControlsPlugin : EditorPlugin
{
    const string ROOT_DIR = "res://addons/pingod-controls/";

    /// <summary> Initialization of the plugin goes here. Add controls to the godot editor. </summary>
    public override void _EnterTree()
	{
        //In the editor load customtypes
		if (Engine.IsEditorHint())
        {
            //These can be added to scenes from the editor
            LoadCustomTypes();
        }
    }

    /// <summary> Add custom types to load </summary>
    private void LoadCustomTypes()
    {
        //Icon for godot editor
        using var texture = GD.Load<Texture2D>($"{Paths.PINGOD_ASSETS}img/pinball.png");
        LoadBlinkingLabel(texture);
        LoadBumper(texture);
        LoadSpinner(texture);
        LoadPinballLanes(texture);
        LoadTargetsBank(texture);
        LoadSaucerTimer(texture);
        LoadVideoPlayerPinball(texture);
    }

    private void LoadTargetsBank(Texture2D texture)
    {
        var script = GD.Load<Script>(ROOT_DIR + $"Node/{nameof(TargetsBank)}.cs");
        AddCustomType(nameof(TargetsBank), nameof(Node), script, texture);
        Logger.Debug(nameof(PinGodControlsPlugin),
            $":{nameof(TargetsBank)}. ADDED");
    }

    private void LoadBumper(Texture2D texture)
    {
        var script = GD.Load<Script>(ROOT_DIR + $"Node/{nameof(Bumper)}.cs");
        AddCustomType(nameof(Bumper), nameof(Node), script, texture);
        Logger.Debug(nameof(PinGodControlsPlugin),
            $":{nameof(Bumper)}. ADDED");
    }

    private void LoadPinballLanes(Texture2D texture)
    {
        var script = GD.Load<Script>(ROOT_DIR + $"Node/{nameof(PinballLanes)}.cs");
        AddCustomType(nameof(PinballLanes), nameof(Node), script, texture);
        Logger.Debug(nameof(PinGodControlsPlugin),
            $":{nameof(PinballLanes)}. ADDED");
    }


    private void LoadSpinner(Texture2D texture)
    {
        var script = GD.Load<Script>(ROOT_DIR + $"Node/{nameof(Spinner)}.cs");
        AddCustomType(nameof(Spinner), nameof(Node), script, texture);
        Logger.Debug(nameof(PinGodControlsPlugin),
            $":{nameof(Spinner)}. ADDED");
    }

    private void LoadSaucerTimer(Texture2D texture)
    {
        var script = GD.Load<Script>(ROOT_DIR + $"Timer/{nameof(Saucer)}.cs");
        AddCustomType(nameof(Saucer), nameof(Timer), script, texture);
        Logger.Debug(nameof(PinGodControlsPlugin),
            $":{nameof(Saucer)}. ADDED");
    }

    private void LoadBlinkingLabel(Texture2D texture)
    {
        var script = GD.Load<Script>(ROOT_DIR + $"Label/{nameof(BlinkingLabel)}.cs");
        AddCustomType(nameof(BlinkingLabel), nameof(Label), script, texture);
        Logger.Debug(nameof(PinGodControlsPlugin),
            $":{nameof(BlinkingLabel)}. ADDED");
    }

    private void LoadVideoPlayerPinball(Texture2D texture)
    {
        var script = GD.Load<Script>(ROOT_DIR + $"VideoPlayer/{nameof(VideoPlayerPinball)}.cs");
        AddCustomType(nameof(VideoPlayerPinball), nameof(VideoStreamPlayer), script, texture);
        Logger.Debug(nameof(PinGodControlsPlugin),
            $":{nameof(VideoPlayerPinball)} ADDED ");
    }

    /// <summary> Clean-up of the plugin goes here. </summary>
    public override void _ExitTree()
	{
        RemoveCustomType(nameof(BlinkingLabel));
        RemoveCustomType(nameof(Bumper));        
        RemoveCustomType(nameof(Saucer));
        RemoveCustomType(nameof(TargetsBank));
        RemoveCustomType(nameof(VideoPlayerPinball));
    }
}
#endif
