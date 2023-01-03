#if TOOLS
using Godot;

[Tool]
public partial class PinGodMachinePlugin : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "addons/pingod-machine/";
    const string VERSION = "1.0";

    /// <summary>
    /// Init plugin if in editor.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        Logger.Debug(nameof(PinGodMachinePlugin), nameof(_EnterTree),":"+ROOT_DIR);
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(PinGodMachinePlugin), ":" + nameof(_EnterTree));
        }
        else
        {            
            //add Bumper:Node . Select in editor add to scene
            using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");

            var script = GD.Load<Script>(ROOT_DIR + $"tools/{nameof(Bumper)}.cs");
            AddCustomType(nameof(Bumper), nameof(Node), script, texture);
            Logger.Debug(nameof(PinGodMachinePlugin),$":TOOL: {nameof(Bumper)}. USAGE: add to scene and set Coil Name and Switch Name");

            script = GD.Load<Script>(ROOT_DIR + $"tools/{nameof(TargetsBank)}.cs");
            AddCustomType(nameof(TargetsBank), nameof(Node), script, texture);
            Logger.Debug(nameof(PinGodMachinePlugin), $":TOOL: {nameof(TargetsBank)}. USAGE: add to scene and set target switch names, hook to OnTargetsCompleted, OnTargetActivated");

            var scenePath = "res://autoload/Machine.tscn";
            AddAutoloadSingleton("Machine", scenePath);
            Logger.Info(nameof(PinGodMachinePlugin), $": Autoload {scenePath}. Access scene from node /root/Machine");
        }        
    }

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug(nameof(PinGodMachinePlugin),nameof(_Ready));
    }

    /// <summary>
    /// clean up goes here. remove custom types
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(PinGodMachinePlugin), nameof(_ExitTree));
        }
        else 
        {
            Logger.Debug(nameof(PinGodMachinePlugin), $":{nameof(_ExitTree)} - editor, removing custom type");
            RemoveCustomType(nameof(Bumper));
            RemoveAutoloadSingleton("Machine");
        }
    }
}
#endif