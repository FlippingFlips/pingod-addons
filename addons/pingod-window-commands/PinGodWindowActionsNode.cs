using Godot;
using System.Drawing;

/// <summary>
/// Handles input from the window. Converts actions to switches <see cref="_gameWindowSwitches"/>
/// </summary>
public partial class PinGodWindowActionsNode : Node
{
    [Export] bool _standardInputHandlingOn = true;
    [Export] bool _sendPingodMachineSwitches = true;
    [Export] string[] _gameWindowSwitches = null;

    private PinGodMachine _machine;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        Logger.Log(LogLevel.Info, Logger.BBColor.green, nameof(PinGodWindowActionsNode), ":" + nameof(_EnterTree));

        if (!Engine.IsEditorHint())
        {
            if (_sendPingodMachineSwitches)
            {
                if (HasNode("/root/Machine"))
                {
                    Logger.Debug(nameof(PinGodWindowActionsNode), $": {nameof(PinGodMachine)} found in Tree");                    
                    _machine = GetNode<PinGodMachine>("/root/Machine");
                }
                else
                {
                    Logger.WarningRich(nameof(PinGodWindowActionsNode), ": [color=yellow]", $"/root/Machine not found. This module uses Machine SwitchActionOff/On.", "[/color]");
                    _sendPingodMachineSwitches = false;
                }
            }
            else { Logger.Debug(nameof(PinGodWindowActionsNode), ":Machine switch handling off"); }
        }        
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        var name = @event.ResourceName;
        if (!@event.IsActionType()) return;
        if (@event is InputEventMouse) return;

        if (_standardInputHandlingOn)
        {
            //quits the game. ESC
            if (InputMap.HasAction("quit"))
            {
                if (@event.IsActionPressed("quit"))
                {
                    Quit();
                    return;
                }
            }

            if (InputMap.HasAction("toggle_border"))
            {
                if (@event.IsActionPressed("toggle_border"))
                {
                    ToggleBorder();
                };
            }
        }

        /*
 * keyboard modifiers
if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.CtrlPressed)
{
    switch ((Key)keyEvent.Keycode)
    {
        case Key.T:
            GD.Print(keyEvent.CtrlPressed ? "Ctrl+T was pressed" : "T was pressed");
            break;
    }
}
*/

        //action to switch. if this event is an action go through the actions set in the game window switches
        if (_sendPingodMachineSwitches)
        {
            if (_machine != null && _gameWindowSwitches?.Length > 0)
            {
                foreach (var sw in _gameWindowSwitches)
                {
                    if (_machine.SwitchActionOn(sw, @event))
                        break;
                    if (_machine.SwitchActionOff(sw, @event))
                        break;
                    //todo: done? need to grab the machineConfig in PinGodGame?
                    //SwitchActionOn(sw, @event);                
                }
            }
        }        
    }

    private static void ToggleBorder()
    {
        Display.ToggleWinFlag(
            DisplayServer.WindowFlags.ResizeDisabled | 
            DisplayServer.WindowFlags.Borderless);
    }

    private void Quit()
    {
        Logger.Debug(nameof(PinGodWindowActionsNode), ":quit action request. quitting whole tree.");
        //todo: commented out, is it needed? :)
        //SetGameResumed();
        GetTree().Quit(0);
    }
}