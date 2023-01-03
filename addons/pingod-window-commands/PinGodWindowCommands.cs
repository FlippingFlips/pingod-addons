using Godot;
using System.IO;

/// <summary>
/// AudioManager node
/// </summary>
[Tool]
public partial class PinGodWindowCommands : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "addons/pingod-window-commands/";
    const string VERSION = "1.0";

    /// <summary>
    /// Initialization of the PlugIn. Adds CustomTypes new type with a name, a parent type, a script and an icon.
    /// </summary>
    public override void _EnterTree()
	{
        Logger.Debug(nameof(PinGodWindowCommands), nameof(_EnterTree), ":" + ROOT_DIR + $":version:{VERSION}");
        if (Engine.IsEditorHint())
        {
            /*
            using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");
            var script = GD.Load<Script>(ROOT_DIR + nameof(PinGodWindowActionsNode) + ".cs");
            AddCustomType(nameof(PinGodWindowActionsNode), nameof(Node), script, texture);
            */

            var scenePath = $"res://autoload/WindowActions.tscn";
            if (Godot.FileAccess.FileExists(scenePath))
            {
                Logger.Debug(nameof(PinGodWindowCommands), ":" + nameof(_EnterTree), $" ");                
                AddAutoloadSingleton("WindowActions", scenePath);
                Logger.Info(nameof(PinGodWindowCommands), $": Autoload " + scenePath, ". Access scene from node /root/WindowActions");
            }
            else
            {
                Logger.WarningRich("[color=yellow]", nameof(PinGodWindowCommands), ": Create scene file with " + nameof(PinGodWindowActionsNode) + " as a base script at"  + scenePath, "[/color]");
            }
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
        Logger.Debug(nameof(PinGodWindowCommands), ":" + nameof(_ExitTree), " removing types");
        //RemoveCustomType(nameof(PinGodWindowActionsNode));
        RemoveAutoloadSingleton("WindowActions");
    }
}