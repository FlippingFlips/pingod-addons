using Godot;
using PinGod.Base;
using PinGod.Core.Nodes.PlungerLane;
using PinGod.Modes;
using System;
using System.Linq;
using PinGod.EditorPlugins;

namespace PinGod.Core.Service
{
    /// <summary>
    /// Machine Godot Node. Holds all machine items before runtime. Adds to the static collections in <see cref="MachineNode"/>
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

        /// <summary>
        /// How long to wait for ball searching and reset
        /// </summary>
        [Export] private int _ball_search_wait_time_secs = 10;

        [ExportCategory("Machine Items")]
        [Export] Godot.Collections.Dictionary<string, byte> _coils = new();
        [Export] Godot.Collections.Dictionary<string, byte> _lamps = new();
        [Export] Godot.Collections.Dictionary<string, byte> _leds = new();        
        [Export] Godot.Collections.Dictionary<string, byte> _switches = new();        

        [ExportCategory("Switch Window")]
        [Export] bool _switchWindowEnabled = false;
        [Export] PackedScene _switchWindow;

        [ExportCategory("Record / Playback")]
        [Export] RecordPlaybackOption recordPlayback = RecordPlaybackOption.Off;
        [Export(PropertyHint.GlobalFile, "*.record")] string _playbackfile = null;
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
        ulong _machineLoadTime;
        private MemoryMapNode _pinGodMemoryMapNode;
        private EventRecordFile _recordFile;
        private RecordPlaybackOption _recordPlayback;
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
        /// <see cref="AddCustomMachineItems"/>
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

                _recordFile = new EventRecordFile();

                //ball search options
                BallSearchOptions = new BallSearchOptions(_ball_search_coils, _ball_search_stop_switches, _ball_search_wait_time_secs, _ball_search_enabled);

                //adds machine items and actions
                AddCustomMachineItems(_coils, _switches, _lamps, _leds);

                if (GetParent().HasNode("MemoryMap"))
                {
                    _pinGodMemoryMapNode = GetParent().GetNode<MemoryMapNode>("MemoryMap");
                }

                //create and add a ball search timer
                BallSearchTimer = new Timer() { Autostart = false, OneShot = false };
                BallSearchTimer.Connect("timeout", new Callable(this, nameof(OnBallSearchTimeout)));
                this.AddChild(BallSearchTimer);

                if (HasNode("BallSaver"))
                {
                    _ballSaver = GetNode<BallSaver>("BallSaver");
                    _ballSaver.BallSaved += _ballSaver_BallSaved;
                }
                if (HasNode("PlungerLane"))
                {
                    _plungerLane = GetNode<PlungerLane>("PlungerLane");
                }
            }
        }

        /// <summary>
        /// Saves recordings if enabled and runs <see cref="Quit(bool)"/>
        /// </summary>
        public override void _ExitTree()
        {
            if (_recordPlayback == RecordPlaybackOption.Record)
            {
                Logger.Info(nameof(MachineNode), ":_ExitTree, saving recordings");
            }
            else Logger.Info(nameof(MachineNode), ":_ExitTree");
        }

        /// <summary>
        /// Process playback events. Turns off if playback is switched off.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            base._Process(delta);
            if (_recordPlayback != RecordPlaybackOption.Playback)
            {
                SetProcess(false);
                Logger.Info(nameof(MachineNode), ": Playback _Process loop stopped. No recordings are being played back.");
                return;
            }
            else
            {
                var cnt = _recordFile.GetQueueCount();
                if (cnt <= 0)
                {
                    Logger.Info(nameof(MachineNode), ": playback events ended, RecordPlayback is off.");
                    _recordPlayback = RecordPlaybackOption.Off;
                    _recordFile.SaveRecording();
                    return;
                }


                var evt = _recordFile.ProcessQueue(_machineLoadTime);
                if (evt != null)
                    SetSwitch(evt.EvtName, evt.State, false);
            }
        }

        public override void _Ready()
        {
            base._Ready();
            if (_pinGodMemoryMapNode != null)
            {
                //listen for events from a memory map
                _pinGodMemoryMapNode.MemorySwitchSignal += OnSwitchCommand;
                Logger.Debug(nameof(MachineNode), ":listening for events from plug-in ", nameof(MemoryMapNode));
            }

            if (_switchWindowEnabled && _switchWindow != null)
            {
                CallDeferred(nameof(SetUpSwitchWindow));
            }
            else
            {
                Logger.Debug(nameof(MachineNode), ": switch window not enabled or scene isn't set");
            }

            //set start time
            _machineLoadTime = Godot.Time.GetTicksMsec();
            //set up recording / playback from [export] properties
            SetUpRecordingsOrPlayback(recordPlayback, _playbackfile);
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
                    Logger.Debug(nameof(IPinGodGame), ":pulsing search coils");
                    for (int i = 0; i < BallSearchOptions?.SearchCoils.Length; i++)
                    {
                        CoilPulse(BallSearchOptions?.SearchCoils[i], 255);
                    }
                }
            }

            BallSearchTimer?.Stop();
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
        public void SetSwitch(string name, byte value, bool fromAction = true)
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
        public void SetSwitch(Switch @switch, byte value, bool fromAction = true)
        {
            //Logger.Verbose($"set switch {@switch.Num}, from godot action?:" + fromAction);
            if (!fromAction)
                @switch.SetSwitch(value);

            //record switch
            if (_recordPlayback == RecordPlaybackOption.Record)
            {
                _recordFile.RecordEventByName(@switch, _machineLoadTime);
            }

            ProcessSwitch(@switch);
            EmitSignal(nameof(SwitchCommand), @switch.Name, @switch.Num, value);
        }

        public virtual void SetUpRecordingsOrPlayback(RecordPlaybackOption playbackOption, string playbackfile)
        {
            _recordPlayback = playbackOption;

            Logger.Debug(nameof(MachineNode), ":setup playback?: ", _recordPlayback.ToString());
            if (_recordPlayback == RecordPlaybackOption.Playback)
            {
                if (string.IsNullOrWhiteSpace(playbackfile))
                {
                    Logger.Warning(nameof(MachineNode), ":", nameof(SetUpRecordingsOrPlayback), ": playback enabled but no record file set.");
                    _recordPlayback = RecordPlaybackOption.Off;
                }
                else
                {
                    try
                    {
                        if (_recordFile.PopulateQueueFromPlaybackFile(playbackfile) == Error.Ok)
                        {
                            Logger.Info(nameof(MachineNode), ":running playback file: ", playbackfile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"playback file failed: " + ex.Message);
                    }
                }
            }
            else if (_recordPlayback == RecordPlaybackOption.Record)
            {
                _recordFile.StartRecording(playbackfile);
                Logger.Debug(nameof(MachineNode), ":game recording on");
            }
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
            var result = sw.IsActionOff(inputEvent);
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

        /// <summary>
        /// Adds custom machine items. Actions are created for switches if they do not exist
        /// </summary>
        /// <param name="coils"></param>
        /// <param name="switches"></param>
        /// <param name="lamps"></param>
        /// <param name="leds"></param>
        protected void AddCustomMachineItems(Godot.Collections.Dictionary<string, byte> coils, Godot.Collections.Dictionary<string, byte> switches, Godot.Collections.Dictionary<string, byte> lamps, Godot.Collections.Dictionary<string, byte> leds)
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
                if (!Godot.InputMap.HasAction("sw" + swVal))
                {
                    Godot.InputMap.AddAction("sw" + swVal);
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
            switches.Clear();

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
            Logger.Debug(nameof(MachineNode), ": ball saved. TODO: act");
            if (_plungerLane != null)
                _plungerLane.AutoFire();
        }
        private void OnSwitchCommand(string name, int index, byte value)
        {
            Logger.Debug(nameof(MachineNode), $": setSwitch--n:{name},numVal:{index}-{value}");
            if (string.IsNullOrWhiteSpace(name))
            {
                SetSwitch(index, value, false);
            }
            else
            {
                SetSwitch(name, value, false);
            }

        }

        private void SetUpSwitchWindow()
        {
            //create window and add scene to instance
            var window = _switchWindow.Instantiate() as WindowPinGod;
            window.InitPackedScene();
            //add window to the root window
            var rootWin = this.GetTree().Root;
            rootWin.GuiEmbedSubwindows = false;
            rootWin.CallDeferred("add_child", window);

            rootWin.GrabFocus();
            //returns the scenes in this root
            var windows = rootWin.GetChildren();//.Where(x => x.GetType() == typeof(Window));
            GD.Print("windows: " + string.Join(',', windows.Select(x => x.Name)));
        }
    }
}