using Godot;
using PinGod.Core;
#if TOOLS

/// <summary>Developer / User can override the named scene or script loaded by placing their own version in the res://autoload <para/>
/// eg: res://autoload/Resources.tscn will load instead of the default plugin
/// </summary>
public abstract partial class PinGodEditorPlugin : EditorPlugin
{
    public string ROOT_DIR;
    public string BaseName { get; }

    /// <summary> Base editorPlugin helper </summary>
    /// <param name="directory">path to plugin files</param>
    /// <param name="name">name should match name of scene eg: Resources. Base Name</param>
    public PinGodEditorPlugin(string directory, string name)
    {
        BaseName = name;
        ROOT_DIR = directory;
    }

    /// <summary> Default plugin scene </summary>
    /// <returns></returns>
    public string AutoLoadScenePath() => ROOT_DIR + BaseName + ".tscn";

    /// <summary> Create path name for over-ridable auto-load scenes <para/>
    /// res://autoload/name.tscn </summary>
    /// <returns></returns>
    public string AutoLoadSceneOverridePath() => $"res://autoload/" + BaseName + ".tscn";

    public override void _EnterTree()
    {
        Logger.Info("PLUGIN-", nameof(PinGodEditorPlugin), $": {ROOT_DIR}");
        base._EnterTree();
        SetAutoLoadPlugin();
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        if(IsAutoLoadSet)
        {
            if (!Engine.IsEditorHint())
            {
                Logger.Debug(BaseName, $":{nameof(_ExitTree)}: removing AutoLoad scene");
            }
            else
            {
                Logger.Debug(BaseName, $":{nameof(_ExitTree)}: removing AutoLoad scene from editor");
            }
            RemoveAutoloadSingleton(BaseName);
        }        
    }

    /// <summary> does nothing, for override purpose for your plugin to set the AutoLoad in Godot editor </summary>
    protected virtual void SetAutoLoadPlugin() { }

    protected bool IsAutoLoadSet = false;

    protected void AddAutoLoad(string path)
    {
        if(!IsAutoLoadSet)
        {
            IsAutoLoadSet = true;
            Logger.Info(nameof(PinGodEditorPlugin), $"found at {path}");
            AddAutoloadSingleton(BaseName, path);
        }        
    }
}
#endif
