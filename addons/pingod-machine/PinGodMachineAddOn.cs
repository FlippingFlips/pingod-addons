using Godot;

/// <summary>
/// This add custom types to use in the Godot editor
/// </summary>
[Tool]
public partial class PinGodMachineAddOn : EditorPlugin
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
            GD.Print(nameof(PinGodMachineAddOn), ":"+nameof(_EnterTree));
        }
        else { GD.Print("enter tree in editor addon"); }

        var script = GD.Load<Script>("addons/PinGod-Machine/MachineConfig.cs");
        
        AddCustomType(nameof(MachineConfig), nameof(Node), script, null);

        //SetAutoLoad();
    }

    private void SetAutoLoad()
    {
        var path = "res://PinGodGame.tscn";
        GD.Print($"setting autoload, looking for {nameof(PinGodMachineAddOn)}. PinGodGame.tscn at {path}");
    }

    public override void _Ready()
    {
        base._Ready();
        GD.Print("pingod game addon ready");
    }

    /// <summary>
    /// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {            
            GD.Print(nameof(PinGodMachineAddOn),":exit tree no editor");
        }
        else { GD.Print(nameof(PinGodMachineAddOn),":exit tree editor"); }
        //RemoveAutoloadSingleton("PinGodGame");
    }
}