using Godot;
using PinGod.Base;
using PinGod.Core.Nodes.PlungerLane;
using System;
using System.Linq;
using PinGod.EditorPlugins;
using PinGod.Core.BallStacks;
using Godot.Collections;
using PinGodAddOns.addons.pingod_machine;
using System.Diagnostics;

namespace PinGod.Core.Service
{
    /// <summary>
    /// Machine Godot Node. Holds all machine items before runtime in the scene file. <para/>
    /// Adds to the static collections in <see cref="Machine"/> </para>
    /// <see cref="Trough"/>, <see cref="MemoryMapNode"/>
    /// </summary>
    public partial class MachineNode : Node
	{
		#region Exports
		[ExportCategory("Ball Search")]
		/// <summary>
		/// Coil names to pulse when ball searching
		/// </summary>
		[Export] public string[] _ball_search_coils;
		/// <summary>
		/// 
		/// </summary>
		[Export] public bool _ball_search_enabled = true;
		/// <summary>
		/// Switches that stop the ball searching
		/// </summary>
		[Export] public string[] _ball_search_stop_switches;
		[ExportCategory("Machine Items")]
		[Export] protected Dictionary<string, byte> _coils = new();

		[Export] protected Dictionary<string, byte> _lamps = new();

		[Export] protected Dictionary<string, byte> _leds = new();

		[Export] protected Dictionary<string, byte> _switches = new();

		/// <summary>
		/// How long to wait for ball searching and reset
		/// </summary>
		[Export] private int _ball_search_wait_time_secs = 10;
		#endregion

		#region Signals
		[Signal] public delegate void CoilPulseTimedOutEventHandler(string name);

		/// <summary>
		/// Emitted when a switch comes into the game. From <see cref="PinGodMemoryMapNode.ReadStates"/> then <see cref="PinGodGame.SetSwitch(int, byte)"/>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
		[Signal] public delegate void SwitchCommandEventHandler(string name, byte index, byte value);
		#endregion

		public BallSaver _ballSaver;
		public PlungerLane _plungerLane;

		static bool _instanceLoaded = false;		

        /// <summary> Hooks onto the OnSwitchCommand if the MemoryMap plugin is available</summary>
        private MemoryMapNode _pinGodMemoryMapNode;

		private RecordingNode _recordingNode;
		
		private Trough _trough;
		/// <summary>
		/// 
		/// </summary>
		public BallSearchOptions BallSearchOptions { get; private set; }
		/// <summary>
		/// Timer node
		/// </summary>
		public Timer BallSearchTimer { get; set; }

		public bool GameInPlay { get; set; }

		/// <summary>
		/// Sets up BallSearch, Machine Items, Ball Saver, plunger lane
		/// </summary>
		public override void _EnterTree()
		{
			if (!Engine.IsEditorHint())
			{
				if (_instanceLoaded)
				{
					Logger.Warning(nameof(MachineNode), $":_EnterTree multiple {nameof(MachineNode)} found, removing..");
					this.QueueFree();
					return;
				}
				_instanceLoaded = true;

				//ball search
				SetupBallSearch();

				//adds machine items and actions
				AddCustomMachineItems(_coils, _switches, _lamps, _leds);

				//memory map (windows)
				if (GetParent().HasNode("MemoryMap"))
                    _pinGodMemoryMapNode = GetParent().GetNode<MemoryMapNode>("MemoryMap");

				if (HasNode(nameof(RecordingNode)))
				{
                    _recordingNode = GetNode<RecordingNode>(nameof(RecordingNode));
                }

                //hook up to the ball saved event
                if (HasNode("BallSaver"))
                {
                    _ballSaver = GetNode<BallSaver>("BallSaver");
                    _ballSaver.BallSaved += _ballSaver_BallSaved;
                }

                //plunger lane
                if (HasNode("PlungerLane")) { _plungerLane = GetNode<PlungerLane>("PlungerLane"); }
			}
		}

		/// <summary>
		/// Saves recordings if enabled and runs <see cref="Quit(bool)"/>
		/// </summary>
		public override void _ExitTree() =>
            Logger.Verbose(nameof(MachineNode), ":_ExitTree");        

		public override void _Ready()
		{
			base._Ready();
			if (_pinGodMemoryMapNode != null)
			{
				//listen for events from a memory map
				_pinGodMemoryMapNode.MemorySwitchSignal += OnSwitchCommand;
				Logger.Debug(nameof(MachineNode), ":listening for events from plug-in ", nameof(MemoryMapNode));
			}

			//get the trough
			if (HasNode("Trough")) _trough = GetNodeOrNull<Trough>("Trough");
		}

		/// <summary>
		/// Creates a <see cref="PulseTimer"/> and adds as child. Pulse timer deal with removing from tree
		/// </summary>
		/// <param name="name"></param>
		/// <param name="pulse">milliseconds. Has an integer here because in simulation, we can use higher if want longer timer</param>
		public virtual void CoilPulse(string name, int pulse = 255)
		{
			Logger.Debug(nameof(MachineNode), $":coil:{name} pulse timer:{pulse}");
			AddChild(new PulseTimer() { Autostart = true, OneShot = true, Name = name, WaitTime = pulse });
		}

        public void DisableBallSaver() => _ballSaver?.DisableBallSave();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True is <see cref="BallSaver"/> is active and not null</returns>
        public bool IsBallSaveActive() => _ballSaver?.IsBallSaveActive() ?? false;

        /// <summary>
        /// Pulse coils in the SearchCoils when ball search times out
        /// </summary>
        public void OnBallSearchTimeout()
		{
			if (BallSearchOptions.IsSearchEnabled)
			{
				if (BallSearchOptions?.SearchCoils?.Length > 0)
				{
					Logger.Debug(nameof(MachineNode), ":pulsing search coils");
					for (int i = 0; i < BallSearchOptions?.SearchCoils.Length; i++)
					{
						CoilPulse(BallSearchOptions?.SearchCoils[i], 255);
					}
				}
			}

			BallSearchTimer?.Stop();
		}

		/// <summary>
		/// Memory map switch event. Coming from external, like a simulator through the memory mapping file
		/// </summary>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public virtual void OnSwitchCommand(string name, int index, byte value)
		{
			Logger.Verbose(nameof(MachineNode), $": setSwitch--n:{name},numVal:{index}-{value}");
			if (string.IsNullOrWhiteSpace(name))
			{
				SetSwitch(index, value, false);
			}
			else
			{
				SetSwitch(name, value, false);
			}
		}

		/// <summary>
		/// handle ball search and recording
		/// </summary>
		/// <param name="sw"></param>
		public virtual void ProcessSwitch(Switch sw, bool gameInPlay = false)
		{
			//do something with ball search if switch needs to
			if (BallSearchOptions.IsSearchEnabled && gameInPlay)
			{
				if (sw.BallSearch != BallSearchSignalOption.None)
				{
					switch (sw.BallSearch)
					{
						case BallSearchSignalOption.Reset:
							SetBallSearchReset();
							break;
						case BallSearchSignalOption.Off:
							SetBallSearchStop();
							break;
						default:
							break;
					}
				}
			}
		}

		/// <summary>
		/// Resets the time for ball searching from <see cref="BallSearchOptions.SearchWaitTime"/>
		/// </summary>
		public virtual void SetBallSearchReset()
		{
			BallSearchTimer?.Start(BallSearchOptions?.SearchWaitTime ?? 10);
		}

		/// <summary>
		/// Stops the <see cref="BallSearchTimer"/>
		/// </summary>
		public virtual void SetBallSearchStop()
		{
			BallSearchTimer?.Stop();
		}

		/// <summary>
		/// Set IsEnabled on the Switch and emits <see cref="PinGodBase.SwitchCommand"/> with number and byte value <para/>
		/// Switch will be set to 0 or 1 but the signal value can be 0-255 <para/>
		/// Anything listening to the <see cref="PinGodBase.SwitchCommand"/> can get the Switch from <see cref="Machine.Switches"/> without checking keys, it will be valid sent from here
		/// </summary>
		/// <param name="swNum"></param>
		/// <param name="value"></param>
		public virtual void SetSwitch(string name, byte value, bool fromAction = true)
		{
			var sw = Machine.Switches[name];
			SetSwitch(sw, value, fromAction);
		}

		/// <summary>
		/// Set IsEnabled on the Switch and emits <see cref="PinGodBase.SwitchCommand"/> with number and byte value <para/>
		/// Switch will be set to 0 or 1 but the signal value can be 0-255 <para/>
		/// Anything listening to the <see cref="PinGodBase.SwitchCommand"/> can get the Switch from <see cref="Machine.Switches"/> without checking keys, it will be valid sent from here
		/// </summary>
		/// <param name="swNum"></param>
		/// <param name="value"></param>
		public void SetSwitch(int swNum, byte value, bool fromAction = true)
		{            
			var sw = Machine.Switches.Values.FirstOrDefault(x => x.Num == swNum);
			if (sw != null)
			{
				SetSwitch(sw, value, fromAction);
			}
		}

		/// <summary>
		/// Sets Machine switch value and Emits <see cref="SwitchCommand"/>. <para/>
		/// Games and plug-ins can hook up to this signal when a switch changes
		/// </summary>
		/// <param name="switch"></param>
		/// <param name="value"></param>
		/// <param name="fromAction">if false, it doesn't set the machine switch value</param>
		public virtual void SetSwitch(Switch @switch, byte value, bool fromAction = true)
		{
			//Logger.Verbose($"set switch {@switch.Num}, from godot action?:" + fromAction);
			if (!fromAction)
				@switch.SetSwitch(value);

            //record the switch
            _recordingNode?.RecordEventByName(@switch);

            ProcessSwitch(@switch);
			EmitSignal(nameof(SwitchCommand), @switch.Name, @switch.Num, value);
		}

        /// <summary> Called when Enter Tree </summary>
        public virtual void SetupBallSearch()
        {
            //ball search options
            BallSearchOptions = new BallSearchOptions(_ball_search_coils, _ball_search_stop_switches, _ball_search_wait_time_secs, _ball_search_enabled);
            //create and add a ball search timer
            BallSearchTimer = new Timer() { Autostart = false, OneShot = false };
            BallSearchTimer.Connect("timeout", new Callable(this, nameof(OnBallSearchTimeout)));
            this.AddChild(BallSearchTimer);
        }

        /// <summary>
        /// Checks a switches input event by friendly name that is in the <see cref="Switches"/> <para/>
        /// "coin", @event
        /// </summary>
        /// <param name="swName"></param>
        /// <param name="inputEvent"></param>
        /// <returns></returns>
        public virtual bool SwitchActionOff(string swName, InputEvent inputEvent)
		{
			var sw = Machine.Switches[swName];
			var result = sw?.IsActionOff(inputEvent) ?? false;
			if (result)
			{
				SetSwitch(sw, 0);
			}
			return result;
		}

		/// <summary>
		/// Use in Godot _Input events. Checks a switches input event by friendly name from switch collection <para/>
		/// "coin", @event
		/// </summary>
		/// <param name="swName"></param>
		/// <param name="inputEvent"></param>
		/// <returns></returns>
		public virtual bool SwitchActionOn(string swName, InputEvent inputEvent)
		{
			var sw = Machine.Switches[swName];
			var result = sw.IsActionOn(inputEvent);
			if (result)
			{
				SetSwitch(sw, 1);
			}
			return result;
		}

        internal void PulseTrough() => _trough?.PulseTrough();

        /// <summary>Record a godot normal godot action, not a switch. Recording game reset.</summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        internal void RecordAction(string action, byte state)
		{
            //record the switch
            _recordingNode?.RecordEventByAction(action, state);
        }

		/// <summary>
		/// Adds custom machine items. Actions are created for switches if they do not exist
		/// </summary>
		/// <param name="coils"></param>
		/// <param name="switches"></param>
		/// <param name="lamps"></param>
		/// <param name="leds"></param>
		protected virtual void AddCustomMachineItems(Dictionary<string, byte> coils,
			Dictionary<string, byte> switches, Dictionary<string, byte> lamps, Dictionary<string, byte> leds)
		{
			foreach (var coil in coils.Keys)
			{
				Machine.Coils.Add(coil, new PinStateObject(coils[coil]));
			}
			var itemAddResult = string.Join(", ", coils.Keys);
			Logger.Debug(nameof(MachineNode), $":added coils {itemAddResult}");
			coils.Clear();

			foreach (var sw in switches.Keys)
			{
				//create an action for the switch if it doesn't exist.
				var swVal = switches[sw];
				if (!InputMap.HasAction("sw" + swVal))
				{
					InputMap.AddAction("sw" + swVal);
				}

				if (BallSearchOptions.StopSearchSwitches?.Any(x => x == sw) ?? false)
				{
					Machine.Switches.Add(sw, new Switch(sw, swVal, BallSearchSignalOption.Off));
				}
				else
				{
					Machine.Switches.Add(sw, new Switch(sw, swVal, BallSearchSignalOption.Reset));
				}
			}

			itemAddResult = string.Join(", ", switches.Keys);
			//LogDebug($"pingod: added switches {itemAddResult}");
			//switches.Clear();

			foreach (var lamp in lamps.Keys)
			{
				Machine.Lamps.Add(lamp, new PinStateObject(lamps[lamp]));
			}
			//itemAddResult = string.Join(", ", lamps.Keys);
			//LogDebug($"pingod: added lamps {itemAddResult}");
			lamps.Clear();

			foreach (var led in leds.Keys)
			{
				Machine.Leds.Add(led, new PinStateObject(leds[led]));
			}
			//LogDebug($"pingod: added leds {string.Join(", ", leds.Keys)}");
			leds.Clear();

			Logger.Debug(nameof(MachineNode), $":Custom items loaded...");
			Logger.Debug(nameof(MachineNode), $":switches={Machine.Switches.Count}:coils={Machine.Coils.Count}:lamps={Machine.Lamps.Count},:leds={Machine.Leds.Count}");
		}		

        private void _ballSaver_BallSaved(bool earlySwitch = false)
        {
            //only pulse early switches
            if (earlySwitch)
            {
                Logger.Debug(nameof(Trough), ": early ball saved, kicking ball.");
				PulseTrough();
            }
        }
    }
}
