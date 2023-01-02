using Godot;

/// <summary>
/// This add custom types to use in the Godot editor
/// </summary>
[Tool]
public partial class MemoryMapPlugin : EditorPlugin
{
    const string ROOT_DIR = "addons/pingod-memorymap/";

    /// <summary>
    /// Create a custom type for Create New Node
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Info(nameof(MemoryMapPlugin), ":" + nameof(_EnterTree) + " pingod-memorymap addon");
        }

        var script = GD.Load<Script>(ROOT_DIR+ nameof(PinGodMemoryMapNode)+".cs");
        using var texture = GD.Load<Texture2D>("res://addons/assets/img/pinball.png");
        AddCustomType(nameof(PinGodMemoryMapNode), nameof(Node), script, texture);                     

        GD.Print("enter tree in editor addon");
        //AddAutoloadSingleton("MemoryMap", "res://addons/pingod-memorymap/MemoryMap.tscn");
    }

    public override void _Ready()
    {
        base._Ready();
        GD.Print("memory map add on ready");
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
        //RemoveAutoloadSingleton("MemoryMap");
        RemoveCustomType(nameof(PinGodMemoryMapNode));
    }
}