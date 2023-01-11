using Godot;
using PinGod.Core;
using PinGod.Core.Service;
using PinGod.EditorPlugins;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

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
    const string WIN_SAVE = "user://playfieldwindow.save";
    private MachineNode _machine;
    public override void _ExitTree()
    {
        base._ExitTree();
        SaveWindowSettings();
    }

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

        SetupMachineNode(playfield_control);

        LoadWindowSettings();
    }

    void LoadWindowSettings()
    {
        if (FileAccess.FileExists(WIN_SAVE))
        {
            using var settingsSave = FileAccess.Open(WIN_SAVE, FileAccess.ModeFlags.Read);
            var obj = JsonSerializer.Deserialize<PlayfieldWindowSave>(settingsSave.GetLine());
            this.Position = new Vector2i(obj.X, obj.Y);
        }
    }

    /// <summary>
    /// When this window receives a button switch from the playfield control
    /// </summary>
    /// <param name="name"></param>
    /// <param name="state">2 = pulse, on then off instant</param>
    void OnPlayfieldSwitchWindow(string name, byte state)
    {
        Logger.Verbose(nameof(PlayfieldWindow), $": playfield window: {name}-{state}");

        if(name == "_record") 
        { 
            
        }
        else if (state == 2)
        {
            _machine.SetSwitch(name, 1, false);
            _machine.SetSwitch(name, 0, false);
        }
        else _machine.SetSwitch(name, state, false);
    }

    void SaveWindowSettings()
    {
        //save the window position
        using var saveGame = FileAccess.Open(WIN_SAVE, FileAccess.ModeFlags.Write);
        var winSave = new PlayfieldWindowSave { X = Position.x, Y = Position.y };
        saveGame.StoreLine(JsonSerializer.Serialize<PlayfieldWindowSave>(winSave));
    }

    private void SetupMachineNode(Control playfield_control)
    {
        _machine = GetNodeOrNull<MachineNode>("/root/Machine");
        if (_machine != null)
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
}

public class PlayfieldWindowSave
{
    public int X { get; set; }
    public int Y { get; set; }
}