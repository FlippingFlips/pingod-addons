using Godot;
using static Godot.WebSocketPeer;
using System.Xml.Linq;
using System.Linq;

public abstract class PinGodSignals
{
    /// <summary>
    /// Emitted signal when game is paused
    /// </summary>
    [Signal] public delegate void GamePausedEventHandler();
    /// <summary>
    /// Emitted signal when game is resumed
    /// </summary>
    [Signal] public delegate void GameResumedEventHandler();
}

/// <summary>
/// Handles input from the window. Converts actions to switches <see cref="_gameWindowSwitches"/>
/// </summary>
public partial class PinGodWindowActions : Node
{
    [Export] string[] _gameWindowSwitches = null;
    private MachineConfig _machineConfig;

    public override void _Ready()
    {
        base._Ready();        
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        if (GetParent().HasNode(nameof(MachineConfig)))
        {
            Logger.Debug(nameof(PinGodWindowActions), $": {nameof(MachineConfig)} found in Tree");
            _machineConfig = GetParent().GetNode<MachineConfig>(nameof(MachineConfig));
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        var name = @event.ResourceName;
        if (!@event.IsActionType()) return;
        if (@event is InputEventMouse) return;
        
        //quits the game. ESC
        if (InputMap.HasAction("quit"))
        {
            if (@event.IsActionPressed("quit"))
            {
                Logger.Debug(nameof(PinGodWindowActions), ":quit action request. quitting whole tree.");
                //send game window ended, not alive
                Machine.SetCoil("alive", 0);
                Logger.Info(nameof(PinGodWindowActions), ":sent game ended coil: alive 0");

                //todo: commented out, is it needed? :)
                //SetGameResumed();
                GetTree().Quit(0);
                return;
            }
        }

        if (InputMap.HasAction("toggle_border"))
        {
            if (@event.IsActionPressed("toggle_border"))
            {                
                Display.ToggleWinFlag(DisplayServer.WindowFlags.Borderless);
                Display.ToggleWinFlag(DisplayServer.WindowFlags.ResizeDisabled);
            };
        }

        if (InputMap.HasAction("pause"))
        {
            if (@event.IsActionPressed("pause"))
            {
                //if (!settingsDisplay?.Visible ?? false)
                if (GetTree().Paused)
                {
                    Logger.Debug(nameof(PinGodWindowActions), ":resume");
                    //SetGameResumed();
                    //pauseLayer.Hide();
                    EmitSignal("GameResumed");
                    GetTree().Paused = false;
                }
                else
                {
                    //OnPauseGame();
                    EmitSignal("GamePaused");
                    GetTree().Paused = false;
                }
                return;
            }
        }

        if (_machineConfig != null && _gameWindowSwitches?.Length > 0)
        {

            foreach (var sw in _gameWindowSwitches)
            {


                if (_machineConfig.SwitchActionOn(sw, @event))
                    break;

                if (_machineConfig.SwitchActionOff(sw, @event))
                    break;
                //todo: done? need to grab the machineConfig in PinGodGame?
                //SwitchActionOn(sw, @event);                
            }
        }

        /*
        if (InputMap.HasAction("pause"))
        {
            if (@event.IsActionPressed("pause"))
            {
                //if (!settingsDisplay?.Visible ?? false)
                if (GetTree().Paused)
                {
                    Logger.Debug(nameof(PinGodGame), ":resume");
                    SetGameResumed();
                    pauseLayer.Hide();
                    GetTree().Paused = false;
                }
                else
                {
                    OnPauseGame();
                }
                return;
            }
        }

        if (InputMap.HasAction("settings"))
        {
            if (@event.IsActionPressed("settings"))
            {
                if (settingsDisplay != null)
                {
                    var visible = !settingsDisplay.Visible;
                    if (visible)
                    {
                        if (!GetTree().Paused)
                            OnPauseGame();
                    }
                    else
                    {
                        OnResumeGame();
                    }
                    settingsDisplay.Visible = visible;
                }
            }
        }
        */


    }

}