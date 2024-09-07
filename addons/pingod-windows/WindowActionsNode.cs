using Godot;
using Godot.Collections;
using PinGod.Base;
using PinGod.EditorPlugins;
using PinGod.Game;
using System.Linq;

namespace PinGod.Core.Service
{
	/// <summary>Handles input from the main window. <para/>
	/// Converts actions to switches using the <see cref="_gameWindowSwitches"/><para/>
	/// This also handles creating a tool pane for opening other windows, <see cref="SetUpToolsWindow"/></summary>
	public partial class WindowActionsNode : Node
	{
		#region Godot Scene Exports
		/// <summary>These are switches you allow to be sent to the window</summary>
		[Export] protected string[] _gameWindowSwitches = null;

		[Export] protected bool _sendPingodMachineSwitches = true;

		[Export] protected bool _standardInputHandlingOn = true;

		[Export] bool _setDisplayFromAdjustments = true;

		[ExportCategory("Dev Tools Panel")]
		[Export] public bool _toolsWindowEnabled = false;
		[Export] PackedScene _toolsWindow;
		/// <summary>If false the window will be separated from the main window</summary>
		[Export] public bool _embedSubWindow = true;

		[ExportCategory("Dev Windows")]
        /// <summary>Collection of windows for the tools window to show / hide</summary>
        [Export] Dictionary<string, PackedScene> _toolsPanewindows;
        #endregion

        private Adjustments _adjustments;
        protected MachineNode _machine;

        private Window _toolsWindowInstance;
        private Window _switchWindowInstance;

        private Dictionary<string, WindowPinGod> _windowsInstances = new();

        #region Godot overrides

        /// <summary>This attempts to retrieve the MachineNode from the path: <see cref="Paths.ROOT_MACHINE"/><para/>
        /// The machine node is needed for interacting with game switches<para/>
        /// This adds an event to <see cref="Root_CloseRequested"/><para/>
        /// <see cref="Node._EnterTree"/></summary>
        public override void _EnterTree()
		{
			base._EnterTree();

			Logger.Log(LogLevel.Info, Logger.BBColor.green, nameof(WindowActionsNode), ":" + nameof(_EnterTree));

			if (!Engine.IsEditorHint())
			{
				if (_sendPingodMachineSwitches)
				{
					if (HasNode(Paths.ROOT_MACHINE))
					{
						Logger.Debug(nameof(WindowActionsNode), $": {nameof(MachineNode)} found in Tree");
						_machine = GetNode<MachineNode>(Paths.ROOT_MACHINE);
					}
					else
					{
						Logger.WarningRich(nameof(WindowActionsNode), ": [color=yellow]", Paths.ROOT_MACHINE + $" not found. This module uses Machine SwitchActionOff/On.", "[/color]");
						_sendPingodMachineSwitches = false;
					}
				}
				else { Logger.Debug(nameof(WindowActionsNode), ":Machine switch handling off"); }

				GetTree().Root.CloseRequested += Root_CloseRequested;
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
				if (InputMap.HasAction("ui_cancel"))
				{
					if (@event.IsActionPressed("ui_cancel"))
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
						{
							Logger.Debug(nameof(WindowActionsNode), $" {sw}:on");
                            break;
                        }							
						if (_machine.SwitchActionOff(sw, @event))
						{
                            Logger.Debug(nameof(WindowActionsNode), $" {sw}:off");
                            break;
                        }                                                   
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

                //TODO: bring some display options back that cannot be done command line
                //if (_setDisplayFromAdjustments)
                //	SetWindowFromAdjustments();
            }

			//set to view the tools panel from the static config
			_toolsWindowEnabled = PinGodGame.PinGodOverrideConfig.ToolsPaneEnabled;

            //set up a tools pane so the developer can enable other windows from it.			
            if (_toolsWindowEnabled && _toolsWindow != null) CallDeferred(nameof(SetUpToolsWindow));
            else { Logger.Debug(nameof(WindowActionsNode), ": switch window not enabled or scene isn't set"); }
        }

		#endregion

		private void Root_CloseRequested() => Quit();

        /// <summary> Creates the <see cref="_toolsWindow"/> </summary>
        private void SetUpToolsWindow()
        {
			//create tools pane, init packed scene will do nothing, the scene is in the scene
			var toolsWinInstance = _toolsWindow.Instantiate() as WindowPinGod;

			//the root of the window
			var winActionsWin = this.GetTree().Root;
            winActionsWin.GuiEmbedSubwindows = _embedSubWindow;
            winActionsWin.CallDeferred("add_child", toolsWinInstance);
            winActionsWin.GrabFocus();
            winActionsWin.CloseRequested += RootWin_CloseRequested;

            (toolsWinInstance as ToolsPanelWindow).ShowHideWindow += ToolsWindowPane_OnShowHideWindow;

            //returns the scenes in this root
            var windows = toolsWinInstance.GetChildren();//.Where(x => x.GetType() == typeof(Window));
			Logger.Info("windows: " + string.Join(',', windows.Select(x => x.Name)));
		}


        /// <summary>callback event from a signal in tools window</summary>
        /// <param name="buttonName"></param>
        /// <param name="show"></param>
        private void ToolsWindowPane_OnShowHideWindow(string buttonName, bool show)
        {
			//check if the developer has added a window under the same name as the button
            if (_toolsPanewindows?.ContainsKey(buttonName) ?? false)
			{
                if (!_windowsInstances.ContainsKey(buttonName))
                {
					var winInstance = _toolsPanewindows[buttonName]
						.Instantiate() as WindowPinGod;

					if (winInstance == null)
					{
                        Logger.WarningRich($"couldn't instantiate the windows scene as a WindowPinGod Node");
                        return;
					}
					else
					{
						//init any scenes that belong to this window
						winInstance.InitPackedScene();

                        var root = this.GetTree().Root;
						root.CallDeferred("add_child", winInstance);
						
						_windowsInstances.Add(buttonName, winInstance);						

                        winInstance.CloseRequested += () => winInstance.Hide();
                    }
                }
				else
				{
					var instance = _windowsInstances[buttonName];
					if( instance != null)
					{
                        if(!instance.Visible && show) instance.Show();
						else instance.Hide();
                    }
                }
            }
			else
			{
				Logger.WarningRich("no windows found in the tools pane window scenes named:" + buttonName);
			}            
		}

        /// <summary> Creates the <see cref="_toolsWindow"/> </summary>
        private void SetUpSwitchWindow()
        {
			////init the packed scene for the switch window
			//var window = _sw.Instantiate() as WindowPinGod;
			//window.InitPackedScene();

			////the root of the window
			//_switchWindowInstance = this.GetTree().Root;
			//_switchWindowInstance.GuiEmbedSubwindows = _embedSubWindow;
			//_switchWindowInstance.CallDeferred("add_child", window);
			//_switchWindowInstance.GrabFocus();
			//_switchWindowInstance.CloseRequested += RootWin_CloseRequested;

			////returns the scenes in this root
			//var windows = _switchWindowInstance.GetChildren();//.Where(x => x.GetType() == typeof(Window));
			//Logger.Info("windows: " + string.Join(',', windows.Select(x => x.Name)));
		}

        private void RootWin_CloseRequested()
        {
			Logger.Debug($"main window closing");
        }

        /// <summary>
        /// Sets the window from (see <see cref="DisplaySettings"/>) found in the (see <see cref="Adjustments"/>)
        /// </summary>
        public virtual void SetWindowFromAdjustments()
		{
			if (_adjustments != null)
			{
				Display.SetSize(_adjustments.Display.Width, _adjustments.Display.Height);
				Display.SetPosition(_adjustments.Display.X, _adjustments.Display.Y);
			}
		}

		/// <summary>Toggles the border on the main game window</summary>
		public virtual void ToggleBorder()
		{
			//DisplaySettings.ToggleWinFlag(
			//    DisplayServer.WindowFlags.ResizeDisabled |
			//    DisplayServer.WindowFlags.Borderless);
			Display.ToggleWinFlag(DisplayServer.WindowFlags.Borderless);
		}

        /// <summary>Runs quit on the PinGodGame</summary>
        public virtual void Quit()
		{
			Logger.Info(nameof(WindowActionsNode), ":quit action request. quitting tree.");
			//free here to make sure the _exit method is invoked on the scripts
			foreach (var item in GetTree().Root.GetChildren())
			{
				if (item.Name == "PinGodGame")
				{
					//need to do this as it doesn't quit itself
					var pg = item as IPinGodGame;
					pg?.Quit();
				}
				else
				{
					item.QueueFree();
				}
			}
			//quit this window
			GetTree().Quit(0);
		}
	}
}
