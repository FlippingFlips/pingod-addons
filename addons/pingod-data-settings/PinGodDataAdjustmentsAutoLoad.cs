#if TOOLS
using Godot;

[Tool]
public partial class PinGodDataAdjustmentsAutoLoad : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "res://addons/pingod-data-settings/";
    const string VERSION = "1.0";

    /// <summary>
    /// Init plugin if in editor.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        Logger.Debug(nameof(PinGodDataAdjustmentsAutoLoad), nameof(_EnterTree),":"+ROOT_DIR);
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(PinGodDataAdjustmentsAutoLoad), ":" + nameof(_EnterTree));
        }
        else
        {            
            var scenePath = ROOT_DIR+ "Adjustments.tscn";
            AddAutoloadSingleton("Adjustments", scenePath);
            Logger.Info(nameof(PinGodDataAdjustmentsAutoLoad), $": Autoload {scenePath}. Access scene from node /root/Adjustments");

            scenePath = ROOT_DIR + "Audits.tscn";
            AddAutoloadSingleton("Audits", scenePath);
            Logger.Info(nameof(PinGodDataAdjustmentsAutoLoad), $": Autoload {scenePath}. Access scene from node /root/Audits");
        }        
    }

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug(nameof(PinGodDataAdjustmentsAutoLoad),nameof(_Ready));
    }

    /// <summary>
    /// clean up goes here. remove custom types
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        if (!Engine.IsEditorHint())
        {
            Logger.Debug(nameof(PinGodDataAdjustmentsAutoLoad), nameof(_ExitTree));
        }
        else 
        {
            Logger.Debug(nameof(PinGodDataAdjustmentsAutoLoad), $":{nameof(_ExitTree)} - editor, removing autoloads.");
            RemoveAutoloadSingleton("Adjustments");
            RemoveAutoloadSingleton("Audits");
        }
    }
}
#endif