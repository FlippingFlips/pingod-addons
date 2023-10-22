using Godot;
using PinGod.Base;
using PinGod.Core;
using PinGod.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinGod.EditorPlugins
{
    [Tool]
    /// <summary>
    /// SwitchOverlay GridContainer. Creates buttons from Machine.Switches and connects to button events to fire switches off
    /// </summary>
    public partial class SwitchOverlay : GridContainer
    {
        private Switches _switches;
        private List<Button> _troughButtons;

        [Export] protected int _buttonWidth = 50;
        [Export] protected int _buttonHeight = 5;
        private MachineNode _machine;

        /// <summary>
        /// Creates switches from the <see cref="Machine.Switches"/>
        /// </summary>
        public override void _EnterTree()
        {
            if (!Engine.IsEditorHint())
            {
                _switches = Machine.Switches;//?.Values.OrderBy(x => x.Num);
                _troughButtons = new List<Button>();
                //create a button from all switches
                foreach (var sw in _switches.Values.OrderBy(x => x.Num))
                {
                    var btn = CreateSwitchButton(sw);
                    if (sw.Name.Contains("trough"))
                        _troughButtons.Add(btn);
                    AddChild(btn);
                }

                if (HasNode(Paths.ROOT_MACHINE)) _machine = GetNodeOrNull<MachineNode>(Paths.ROOT_MACHINE);
            }
            else
            {
                //Fake switches for viewing in godot editor
                foreach (var item in GetChildren())
                {
                    item.QueueFree();
                }
                for (int i = 0; i < 64; i++)
                {
                    AddChild(CreateSwitchButton(new Switch() { Name = $"Switch" + i, Num = (byte)i }));
                }
            }
        }

        protected virtual Button CreateSwitchButton(Switch sw, int width = 50, int height = 5)
        {
            var button = new Button() { Text = sw.Name, ToggleMode = true };
            button.CustomMinimumSize = new Vector2(width, height);
            button.Toggled += ((pressed) => OnToggle(pressed, sw.Name));
            return button;
        }

        /// <summary>
        /// Runs SetSwitch on the Machine.Switches
        /// </summary>
        /// <param name="button_pressed"></param>
        /// <param name="swName"></param>
        protected virtual void OnToggle(bool button_pressed, string swName)
        {
            if (Engine.IsEditorHint()) return;
            Logger.Verbose("switch overlay: " + swName + button_pressed);
            var state = (byte)(button_pressed ? 1 : 0);

            //use the machine node to send switch and emit signal
            //set not from action (false) when setting switch here to set the switch not just emit signal
            if (_machine != null) _machine.SetSwitch(swName, state, false);
            else Machine.SetSwitch(swName, state);
        }

        internal void ActivateTrough()
        {
            if (_troughButtons?.Count > 0)
            {
                foreach (var btn in _troughButtons)
                {
                    btn.ButtonPressed = true;
                }
            }
        }
    }
}
