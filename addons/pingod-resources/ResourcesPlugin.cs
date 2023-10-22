using Godot;
using PinGod.Core;

#if TOOLS

/// <summary> This adds a Resources singleton when plugin enabled in Godot editor </para></summary>
[Tool]
public partial class ResourcesPlugin : PinGodEditorPlugin
{
    public ResourcesPlugin() : base("res://addons/pingod-resources/", "Resources") { }

    public override void _EnterTree() => base._EnterTree();
    public override void _ExitTree() => base._ExitTree();

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug(nameof(ResourcesPlugin), nameof(_Ready));
    }

    protected override void SetAutoLoadPlugin()
    {
        //assign the scene when in editor
        if (Engine.IsEditorHint())
        {
            var overrideScene = AutoLoadSceneOverridePath();
            Logger.Debug(nameof(ResourcesPlugin), $" looking for scene at {overrideScene}");
            if (FileAccess.FileExists(overrideScene))
            {
                AddAutoLoad(overrideScene);
                return;
            }

            var path = AutoLoadScenePath();            
            if (FileAccess.FileExists(path))
            {
                AddAutoLoad(path);
            }
            else { Logger.Warning("autoload for Resources.tscn could be set.", nameof(Resources), "WARNING: failed to set autoload " + path); }
        }
    }    
}
#endif
