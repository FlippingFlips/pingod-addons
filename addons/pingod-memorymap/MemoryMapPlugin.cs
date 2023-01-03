using Godot;

/// <summary>
/// This add custom types to use in the Godot editor
/// </summary>
[Tool]
public partial class MemoryMapPlugin : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "addons/pingod-memorymap/plugin/";

    /// <summary>
    /// Create a custom type for Create New Node
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        Logger.Debug(nameof(MemoryMapPlugin), nameof(_EnterTree), ":" + ROOT_DIR);
        //we're in Godot editor, load main script and a texture so it's available in the UI
        var script = GD.Load<Script>(ROOT_DIR + nameof(PinGodMemoryMapNode) + ".cs");
        using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");
        AddCustomType(nameof(PinGodMemoryMapNode), nameof(Node), script, texture);
        Logger.Debug(nameof(MemoryMapPlugin), ":" + nameof(_EnterTree), " loaded plugin script");

        //option to add as auto loaded with the scene, scene needs to be configured
        AddAutoloadSingleton("MemoryMap", "res://autoload/MemoryMap.tscn");
    }

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug(nameof(MemoryMapPlugin), nameof(_Ready));
    }

    /// <summary>
    /// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(MemoryMapPlugin), $":{nameof(_ExitTree)}");
        }
        else { 
            Logger.Debug(nameof(MemoryMapPlugin), $":{nameof(_ExitTree)} - editor, removing custom type");            
        }

        RemoveCustomType(nameof(PinGodMemoryMapNode));
        RemoveAutoloadSingleton("MemoryMap");        
    }
}