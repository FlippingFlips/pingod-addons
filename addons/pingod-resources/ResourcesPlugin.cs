using Godot;

/// <summary>
/// This add custom types to use in the Godot editor
/// </summary>
[Tool]
public partial class ResourcesPlugin : EditorPlugin
{
    const string ROOT_DIR = "addons/pingod-resources/";

    public override void _EnterTree()
    {
        base._EnterTree();
        SetAutoLoad();
    }

    private void SetAutoLoad()
    {
        var path = "res://autoload/Resources.tscn";
        Logger.Debug(nameof(ResourcesPlugin), $"setting autoload scene at {path}");

        //assign the scene when in editor
        if (Engine.IsEditorHint())
        {
            if (FileAccess.FileExists(path))
            {
                Logger.Info(nameof(ResourcesPlugin), $"found at {path}");
                AddAutoloadSingleton("PinGod" + nameof(Resources), path);
            }
            else if (FileAccess.FileExists("res://" +ROOT_DIR+"/Resources.tscn"))
            {
                path = "res://" + ROOT_DIR + "/Resources.tscn";
                Logger.Info(nameof(ResourcesPlugin), $"found {path}");
                AddAutoloadSingleton("PinGod"+nameof(Resources), path);
            }
            else { Logger.Warning("autoload for Resources.tscn could be set.", nameof(ResourcesPlugin), "WARNING: failed to set autoload " + path); }
        }        
    }

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug(nameof(ResourcesPlugin), nameof(_Ready));
    }

    /// <summary>
    /// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(ResourcesPlugin), $":{nameof(_ExitTree)}: removing AutoLoad scene");
        }
        else 
        {
            Logger.Debug(nameof(ResourcesPlugin), $":{nameof(_ExitTree)}: removing AutoLoad scene from editor");
        }
        RemoveAutoloadSingleton("PinGod"+nameof(Resources));
    }
}