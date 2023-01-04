using Godot;

/// <summary>
/// Just instance the SettingsDisplay.tscn for this one. Not a real plug-in just separated.
/// </summary>
[Tool]
public partial class SettingsMenuPlugin : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "addons/pingod-settings-menu/";
    const string VERSION = "1.0";

    /// <summary>
    /// Initialization of the PlugIn. Adds CustomTypes new type with a name, a parent type, a script and an icon.
    /// </summary>
    public override void _EnterTree()
	{
        Logger.Debug(nameof(SettingsMenuPlugin), nameof(_EnterTree), ":" + ROOT_DIR + $":version:{VERSION}");
        if (Engine.IsEditorHint())
        {
        }
        else
        {

        }
    }

    public override void _Ready()
    {
        base._Ready();
    }

    /// <summary>
    /// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
    /// </summary>
    public override void _ExitTree()
	{
        Logger.Debug(nameof(SettingsMenuPlugin), ":" + nameof(_EnterTree), " removing types");
        //RemoveCustomType(nameof(Trough));
        //RemoveAutoloadSingleton(nameof(Trough));
    }
}