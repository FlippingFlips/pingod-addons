#if TOOLS
using Godot;
using PinGod.Core;

[Tool]
public partial class PinGodConsolePlugin : PinGodEditorPlugin
{
    public PinGodConsolePlugin() : base("res://addons/pingod-console/", "PinGodConsole") { }

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
            Logger.Debug(nameof(PinGodConsolePlugin), $" looking for scene at {overrideScene}");
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
                Logger.Warning("autoload for PinGodConsolePlugin.tscn could be set.",
                nameof(PinGodConsolePlugin), "WARNING: failed to set autoload " + path);
            }
        }
    }
}
#endif
