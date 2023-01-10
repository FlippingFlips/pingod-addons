using Godot;
using Godot.Collections;
using PinGod.Base;
using System;

namespace PinGod.Core.Service
{
    /// <summary>
    /// Handles input from the window. Converts actions to switches <see cref="_gameWindowSwitches"/>
    /// </summary>
    public partial class WindowActionsNode : Node
    {        
        [Export] string[] _gameWindowSwitches = null;        
        [Export] bool _sendPingodMachineSwitches = true;
        [Export] bool _standardInputHandlingOn = true;
        [Export] bool _setDisplayFromAdjustments = true;        

        private Adjustments _adjustments;
        private MachineNode _machine;

        /// <summary>
        /// Window manager
        /// </summary>
        private Dictionary<string, Window> _windows = new Dictionary<string, Window>();

        #region Godot overrides
        public override void _EnterTree()
        {
            base._EnterTree();
            Logger.Log(LogLevel.Info, Logger.BBColor.green, nameof(WindowActionsNode), ":" + nameof(_EnterTree));

            if (!Engine.IsEditorHint())
            {
                if (_sendPingodMachineSwitches)
                {
                    if (HasNode("/root/Machine"))
                    {
                        Logger.Debug(nameof(WindowActionsNode), $": {nameof(MachineNode)} found in Tree");
                        _machine = GetNode<MachineNode>("/root/Machine");
                    }
                    else
                    {
                        Logger.WarningRich(nameof(WindowActionsNode), ": [color=yellow]", $"/root/Machine not found. This module uses Machine SwitchActionOff/On.", "[/color]");
                        _sendPingodMachineSwitches = false;
                    }
                }
                else { Logger.Debug(nameof(WindowActionsNode), ":Machine switch handling off"); }

                GetTree().Root.CloseRequested += Root_CloseRequested;
            }
        }

        private void Root_CloseRequested()
        {
            Quit();
        }

        /// <summary>
        /// Kill the windows
        /// </summary>
        public override void _ExitTree()
        {
            foreach (var win in _windows?.Values)
            {
                win.QueueFree();
            }

            base._ExitTree();
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

            //action to switch. if this event is an action, go through the actions set in the game window switches
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

        /// <summary>
        /// Sets the window from adjustments
        /// </summary>
        public override void _Ready()
        {
            base._Ready();
            if (HasNode("/root/Adjustments"))
            {
                _adjustments = GetNodeOrNull<AdjustmentsNode>("/root/Adjustments")?._adjustments;
                if (_setDisplayFromAdjustments)
                    SetWindowFromAdjustments();
            }
        }

        #endregion

        /// <summary>
        /// Sets the window from (see <see cref="DisplaySettings"/>) found in the (see <see cref="Adjustments"/>)
        /// </summary>
        public virtual void SetWindowFromAdjustments()
        {
            if (_adjustments != null)
            {
                DisplayServer.WindowSetSize(new Vector2i(_adjustments.Display.Width, _adjustments.Display.Height));
                DisplayServer.WindowSetPosition(new Vector2i(_adjustments.Display.X, _adjustments.Display.Y));
            }
        }

        /// <summary>
        /// Toggles the border and resize
        /// </summary>
        private static void ToggleBorder()
        {
            //DisplaySettings.ToggleWinFlag(
            //    DisplayServer.WindowFlags.ResizeDisabled |
            //    DisplayServer.WindowFlags.Borderless);
            Display.ToggleWinFlag(DisplayServer.WindowFlags.Borderless);
        }

        private void Quit()
        {
            Logger.Debug(nameof(WindowActionsNode), ":quit action request. quitting tree.");
            //free here to make sure teh _exit method is invoked on the scripts
            foreach (var item in GetTree().Root.GetChildren())
            {
                item.QueueFree();
            }
            //quit this window
            GetTree().Quit(0);
        }
    }
}