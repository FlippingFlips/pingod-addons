#if TOOLS
using Godot;
using PinGod.Core;

/// <summary> Adds Machine Autoload singleton when plugin is enabled in godot editor</summary>
[Tool]
public partial class MachinePlugin : PinGodEditorPlugin
{
    public MachinePlugin() : base("res://addons/pingod-machine/", "Machine") { }

    public override void _EnterTree() => base._EnterTree();

    public override void _ExitTree() => base._ExitTree();

    /// <summary>
    /// Add the autoload singleton, find override first in autoload or plugin default
    /// </summary>
    protected override void SetAutoLoadPlugin()
    {
        //assign the scene when in editor
        if (Engine.IsEditorHint())
        {           
            var overrideScene = AutoLoadSceneOverridePath();
            Logger.Debug(nameof(MachinePlugin), $" looking for scene at {overrideScene}");
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
            else
            {
                Logger.Warning("autoload for Machine.tscn could be set.",
                nameof(MachinePlugin), "WARNING: failed to set autoload " + path);
            }   
        }
    }
}
#endif