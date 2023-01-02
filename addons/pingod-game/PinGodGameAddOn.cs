using Godot;

/// <summary>
/// This add custom types to use in the Godot editor
/// </summary>
[Tool]
public partial class PinGodGameAddOn : EditorPlugin
{
    const string ROOT_DIR = "addons/pingod-game/";

    /// <summary>
    /// Initialization of the PlugIn. Adds CustomTypes new type with a name, a parent type, a script and an icon.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Info("enter tree pingodgame addon");
        }
        else { Logger.Info("enter tree in editor addon"); }

        SetAutoLoad();
    }

    private void SetAutoLoad()
    {
        var path = "res://PinGodGame.tscn";
        GD.Print($"setting autoload, looking for PinGodGame. PinGodGame.tscn at {path}");
        if (FileAccess.FileExists(path))
        {
            Logger.Info($"found PinGodGame at res://PinGodGame.tscn");
            AddAutoloadSingleton("PinGodGame", "res://PinGodGame.tscn");
        }
        else if (FileAccess.FileExists("res://game/PinGodGame.tscn"))
        {
            Logger.Info($"found PinGodGame at res://game/PinGodGame.tscn");
            AddAutoloadSingleton("PinGodGame", "res://game/PinGodGame.tscn");
        }
        else if (FileAccess.FileExists("res://addons/pingod-game/Scenes/PinGodGame.tscn"))
        {
            Logger.Info($"found PinGodGame at res://addons/pingod-game/Scenes/PinGodGame.tscn");
            AddAutoloadSingleton("PinGodGame", "res://addons/pingod-game/Scenes/PinGodGame.tscn");
        }
        else { Logger.Info("WARNING: failed to set autoload PinGodGame.tscn"); }
    }

    public override void _Ready()
    {
        base._Ready();
        Logger.Info("pingod game addon ready");
    }

    /// <summary>
    /// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {            
            Logger.Info("exit tree no editor");
        }
        else { Logger.Info("exit tree editor"); }
        RemoveAutoloadSingleton("PinGodGame");
    }
}