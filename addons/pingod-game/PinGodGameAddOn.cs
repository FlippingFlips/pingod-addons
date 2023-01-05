using Godot;

/// <summary>
/// This add custom types to use in the Godot editor
/// </summary>
[Tool]
public partial class PinGodGameAddOn : EditorPlugin
{
    const string ROOT_DIR = "res://addons/pingod-game/";

    public override void _EnterTree()
    {
        base._EnterTree();
        SetAutoLoad();
    }

    private void SetAutoLoad()
    {
        var path = "res://autoload/PinGodGame.tscn";
        Logger.Debug(nameof(PinGodGameAddOn), $"setting autoload, looking for a PinGodGame.tscn at {path}");

        //assign the scene when in editor
        if (Engine.IsEditorHint())
        {
            if (FileAccess.FileExists(path))
            {
                Logger.Info(nameof(PinGodGameAddOn), $"found PinGodGame at {path}");
                AddAutoloadSingleton(nameof(PinGodGame), path);
            }
            else if (FileAccess.FileExists("res://game/PinGodGame.tscn"))
            {
                path = "res://game/PinGodGame.tscn";
                Logger.Info(nameof(PinGodGameAddOn), $"found PinGodGame at {path}");
                AddAutoloadSingleton(nameof(PinGodGame), path);
            }
            else if (FileAccess.FileExists($"{ROOT_DIR}scenes/PinGodGame.tscn"))
            {
                path = $"{ROOT_DIR}scenes/PinGodGame.tscn";
                Logger.Info(nameof(PinGodGameAddOn), $"found PinGodGame at {path}");
                AddAutoloadSingleton(nameof(PinGodGame), path);
            }
            else { Logger.Warning("autoload for PinGodGame.tscn could be set.", nameof(PinGodGameAddOn), "WARNING: failed to set autoload " + path); }
        }        
    }

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug(nameof(PinGodGameAddOn), nameof(_Ready));
    }

    /// <summary>
    /// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(PinGodGameAddOn), $":{nameof(_ExitTree)}: removing AutoLoad scene");
        }
        else 
        {
            Logger.Debug(nameof(PinGodGameAddOn), $":{nameof(_ExitTree)}: removing AutoLoad scene from editor");
        }
        RemoveAutoloadSingleton(nameof(PinGodGame));
    }
}