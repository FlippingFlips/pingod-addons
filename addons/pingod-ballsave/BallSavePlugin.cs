#if TOOLS
using Godot;

[Tool]
public partial class BallSavePlugin : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "addons/pingod-ballsave/";
    const string VERSION = "1.0";

    /// <summary>
    /// Init plugin if in editor.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        Logger.Debug(nameof(BallSavePlugin), nameof(_EnterTree),":"+ROOT_DIR);
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(BallSavePlugin), ":" + nameof(_EnterTree));
        }
        else
        {            
            //add Bumper:Node . Select in editor add to scene
            using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");

            var script = GD.Load<Script>(ROOT_DIR + $"{nameof(BallSaver)}.cs");
            AddCustomType(nameof(BallSaver), nameof(Timer), script, texture);
            Logger.Debug(nameof(BallSavePlugin),$":TOOL: {nameof(BallSaver)}. USAGE: TODO");

            //var scenePath = "res://autoload/Machine.tscn";
            //AddAutoloadSingleton("Machine", scenePath);
            //Logger.Info(nameof(BallSavePlugin), $": Autoload {scenePath}. Access scene from node /root/Machine");
        }        
    }

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug(nameof(BallSavePlugin),nameof(_Ready));
    }

    /// <summary>
    /// clean up goes here. remove custom types
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(BallSavePlugin), nameof(_ExitTree));
        }
        else 
        {
            Logger.Debug(nameof(BallSavePlugin), $":{nameof(_ExitTree)} - editor, removing custom type");
            RemoveCustomType(nameof(BallSaver));
            //RemoveAutoloadSingleton("Machine");
        }
    }
}
#endif