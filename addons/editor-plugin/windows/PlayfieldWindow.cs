using Godot;
using PinGod.Core;
using PinGod.Core.Service;
using PinGod.EditorPlugins;

/// <summary>
/// This window relies on a child having a Control node which contains playfield image and buttons. <para/>
/// see "res://addons/editor-plugin/machine-plugins/playfield/playfield_control.tscn" <para/>
/// This child control is made just with godot script for a signal when the buttons are pushed. <para/>
/// What you would do for your game is duplicate this scene and make your own from it, replacing the image and button names to your switch names. <para/>
/// You would want to duplicate the scene for this script too and set the switch window in the MachineNode scene. <para/>
/// Defaults to 400x900 size with a playfield at 400x908
/// </summary>
public partial class PlayfieldWindow : WindowPinGod
{
    private MachineNode _machine;

    public override void _Ready()
    {
        base._Ready();
        var playfield_control = GetNodeOrNull<Control>("Control");
        if (playfield_control == null)
        {
            Logger.Error(nameof(PlayfieldWindow), nameof(_Ready), ": no Control found that contains the playfield buttons.");
            this.QueueFree();
            return;
        }
            

        _machine = GetNodeOrNull<MachineNode>("/root/Machine");
        if(_machine != null)
        {
            //connect to switch from the godot gd script
            playfield_control.Connect("switch_active", new Callable(this, nameof(OnPlayfieldSwitchWindow)));
            //have to set the postion as the child is off, this is what we would want in most cases anyway.
            playfield_control.Position = new Vector2(0, 0);            
        }
        else
        {
            Logger.Error(nameof(PlayfieldWindow), nameof(_Ready), ": no MachineNode found, unable to send switches to the game.");
        }
    }

    void OnPlayfieldSwitchWindow(string name, byte state)
    {
        Logger.Verbose(nameof(PlayfieldWindow), $": playfield window: {name}-{state}");

        if(state == 2)
        {
            _machine.SetSwitch(name, 1, false);
            _machine.SetSwitch(name, 0, false);
        }            
        else _machine.SetSwitch(name, state, false);
    }
}
