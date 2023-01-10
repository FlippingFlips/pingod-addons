#if TOOLS
using Godot;
using PinGod.Base;
using PinGod.Core;

[Tool]
public partial class MachinePlugin : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "res://addons/autoload/Machine/";
    const string VERSION = "1.0";
    private Control _bottomPanelCtrl;

    /// <summary>
    /// Init plugin if in editor.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        Logger.Debug(nameof(MachinePlugin), nameof(_EnterTree), ": MachineNodePlugin - " + $"VERSION:{VERSION}");
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(MachinePlugin), ":" + nameof(_EnterTree));
        }
        else
        {            
            //add Bumper:Node . Select in editor add to scene
            using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");

            //var script = GD.Load<Script>(ROOT_DIR + $"tools/{nameof(Bumper)}.cs");
            //AddCustomType(nameof(Bumper), nameof(Node), script, texture);
            //Logger.Debug(nameof(MachinePlugin),$":TOOL: {nameof(Bumper)}. USAGE: add to scene and set Coil Name and Switch Name");

            //script = GD.Load<Script>(ROOT_DIR + $"tools/{nameof(TargetsBank)}.cs");
            //AddCustomType(nameof(TargetsBank), nameof(Node), script, texture);
            //Logger.Debug(nameof(MachinePlugin), $":TOOL: {nameof(TargetsBank)}. USAGE: add to scene and set target switch names, hook to OnTargetsCompleted, OnTargetActivated");

            var scenePath = "res://autoload/Machine.tscn";
            if (FileAccess.FileExists(scenePath))
            {
                AddAutoloadSingleton("Machine", scenePath);
                Logger.Info(nameof(MachinePlugin), $": Autoload {scenePath}. Access scene from node /root/Machine");               
            }
            else
            {
                scenePath = ROOT_DIR + nameof(Machine)+".tscn";
                AddAutoloadSingleton(nameof(Machine), scenePath);
                Logger.Info(nameof(MachinePlugin), $": Autoload {scenePath}. Access scene from node /root/Machine");
                Logger.Info(nameof(MachinePlugin), $": Add a scene in /autoload/Machine.tscn and derive from the machine script to customize the machine.");
            }

            //add panel for Machine
            var sceneFile = ROOT_DIR + "bottom-panel/PinGodBottomPanel.tscn";
            var scene = (GD.Load(sceneFile) as PackedScene).Instantiate() as Control;
            _bottomPanelCtrl = scene;
            AddControlToBottomPanel(_bottomPanelCtrl, "Machine (PinGod)");
        }
    }

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug(nameof(MachinePlugin),nameof(_Ready));
        if (Engine.IsEditorHint())
        {
            //populate switches
        }
    }

    /// <summary>
    /// clean up goes here. remove custom types
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(MachinePlugin), nameof(_ExitTree));
        }
        else 
        {
            Logger.Debug(nameof(MachinePlugin), $":{nameof(_ExitTree)} - editor, removing custom type");
            //RemoveCustomType(nameof(Bumper));
            //RemoveCustomType(nameof(TargetsBank));
            RemoveAutoloadSingleton("Machine");
            RemoveControlFromBottomPanel(_bottomPanelCtrl);
        }
    }
}
#endif