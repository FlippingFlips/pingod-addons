using Godot;

/// <summary>
/// AudioManager node
/// </summary>
[Tool]
public partial class TroughPlugin : EditorPlugin
{
    const string ASSETS_DIR = "res://addons/assets/";
    const string ROOT_DIR = "addons/pingod-trough/";
    const string VERSION = "1.0";

    /// <summary>
    /// Initialization of the PlugIn. Adds CustomTypes new type with a name, a parent type, a script and an icon.
    /// </summary>
    public override void _EnterTree()
	{
        Logger.Debug(nameof(PinGodWindowCommands), nameof(_EnterTree), ":" + ROOT_DIR + $":version:{VERSION}");
        if (Engine.IsEditorHint())
        {
            /*
             * custom type            
            */
            using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");
            var script = GD.Load<Script>(ROOT_DIR + nameof(Trough) + ".cs");
            AddCustomType(nameof(Trough), nameof(Node), script, texture);
            Logger.Debug(nameof(TroughPlugin), ":" + nameof(_EnterTree), " loaded custom types");

            //Logger.Debug(nameof(PinGodWindowCommands), ":" + nameof(_EnterTree), $" ");
            //AddAutoloadSingleton(nameof(Trough), "res://autoload/Trough.tscn");
            //Logger.Info(nameof(PinGodWindowCommands), $": Autoload autoload/Trough.tscn. Access scene from node /root/Trough");
        }
        else
        {

        }
    }

    public override void _Ready()
    {
        base._Ready();
    }

    /// <summary>
    /// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
    /// </summary>
    public override void _ExitTree()
	{
        Logger.Debug(nameof(TroughPlugin), ":" + nameof(_EnterTree), " removing types");
        RemoveCustomType(nameof(Trough));
        //RemoveAutoloadSingleton(nameof(Trough));
    }
}