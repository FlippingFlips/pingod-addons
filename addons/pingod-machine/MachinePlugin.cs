#if TOOLS
using Godot;
using PinGod.Core;

/// <summary> Adds Machine Autoload singleton when plugin is enabled in godot editor</summary>
[Tool]
public partial class MachinePlugin : PinGodEditorPlugin
{
    PackedScene MainPanel = null;
    Control MainPanelInstance;

    public MachinePlugin() : base("res://addons/pingod-machine/", "Machine") 
    {
        MainPanel = ResourceLoader.Load<PackedScene>(base.ROOT_DIR + "/MainScreen-Machine.tscn");
    }    

    public override void _EnterTree()
    {
        base._EnterTree();        

        if (Engine.IsEditorHint())
        {
            var inter = GetEditorInterface();
            inter.SelectFile("MainScreen-Machine.tscn");

            MainPanelInstance = (Control)MainPanel.Instantiate();
            // Add the main panel to the editor's main viewport.
            GetEditorInterface().GetEditorMainScreen().AddChild(MainPanelInstance);
        }

        // Hide the main panel. Very much required.
        _MakeVisible(false);
    }
    public override void _ExitTree()
    {
        base._ExitTree();
        if(MainPanelInstance != null) { MainPanelInstance.QueueFree(); }
    }

    private Control _bottomPanelCtrl;

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

    public override Texture2D _GetPluginIcon()
    {
        return ResourceLoader
            .Load<Texture2D>(Paths.PINGOD_ASSETS + "img/pingod.ico");
    }

    public override string _GetPluginName()
    {
        return "Machine";
    }

    public override bool _HasMainScreen()
    {
        return true;
    }


    public override void _MakeVisible(bool visible)
    {
        if (MainPanelInstance != null)
        {
            MainPanelInstance.Visible = visible;
        }
    }
}
#endif