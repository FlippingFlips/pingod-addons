using Godot;
using PinGod.Base;
using System.Linq;
using Godot.Collections;
using PinGodAddOns.addons.pingod_machine;
using PinGod.Core.Service;
using PinGod.Core;
using PinGod.Game;
using System.Text.Json;

/// <summary>
/// Machine Godot Node. Holds all machine items before runtime in the scene file. <para/>
/// Adds to the static collections in <see cref="Machine"/> </para>
/// <see cref="Trough"/>, <see cref="MemoryMapNode"/>
/// </summary>
public partial class MachineNode : Node
{
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

    /// <summary> Ball saver node </summary>
    public BallSaver _ballSaver = new();

    public Timer troughPulseTimer;

    //public Trough _trough;

    static bool _instanceLoaded = false;

    private bool _isNewBall;

    private IPinGodGame _pingod;

    /// <summary> Hooks onto the OnSwitchCommand if the MemoryMap plugin is available</summary>
    private MemoryMapNode _pinGodMemoryMapNode;

    private RecordingNode _recordingNode;
    public BallSearchOptions BallSearchOptions { get; private set; }

    /// <summary>
    /// Timer node
    /// </summary>
    public Timer BallSearchTimer { get; set; }

    /// <summary>Total balls locked. Included in trough full count</summary>
    public int BallsLocked { get; set; }

    public bool GameInPlay { get; set; }

    /// <summary>loaded machine configuration from a PROC json file</summary>
    public MachineProcJson MachineProcJson { get; private set; }

    #region Godot Overrides
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

            _EnterTreeSetup();
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
            _pinGodMemoryMapNode.MemorySwitchSignal += OnMemorySwitchCommand;
            Logger.Debug(nameof(MachineNode), ":listening for events from plug-in ", nameof(MemoryMapNode));
        }
    }
    #endregion

    /// <summary> Returns the BallsInPlay by checking the amount currently in the trough</summary>
    /// <returns></returns>
    public int BallsInPlay() => _trough_switches.Length - BallsInTrough();

    /// <summary>
    /// Counts the number of trough switches currently active
    /// </summary>
    /// <returns></returns>
    public int BallsInTrough()
    {
        var cnt = 0;
        for (int i = 0; i < _trough_switches.Length; i++)
        {
            if (Machine.Switches[_trough_switches[0]].IsEnabled())
            {
                cnt++;
            }
        }
        return cnt;
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

    /// <summary></summary>
    /// <returns>True is <see cref="BallSaver"/> is active and not null</returns>
    public bool IsBallSaveActive() => _ballSaver?.IsBallSaveActive() ?? false;

    public bool IsPlungerSwitchActive() => Machine.IsSwitchOn((_plunger_lane_switch));

    /// <summary>
    /// Checks every switch in <see cref="TroughOptions.GameSwitches"/> for IsEnabled. <para/>
    /// If <see cref="_isDebugTrough"/> then will return IsFull true
    /// </summary>
    public bool IsTroughFull()
    {
        if (_isDebugTrough)
        {
            Logger.Verbose(nameof(MachineNode), ": DEBUG TROUGH IS ON, always full, switch off for simulator.");
            return true;
        }

        if (_trough_switches?.Length <= 0)
        {
            Logger.Warning(nameof(MachineNode), ":no trough switches set");
            return false;
        }

        var isFull = true;

        for (int i = 0; i < _trough_switches.Length - BallsLocked; i++)
        {
            if (!Machine.Switches[_trough_switches[i]].IsEnabled())
            {
                isFull = false;
                break;
            }
        }

        return isFull;
    }

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

    /// <summary>Memory map switch event. Coming from external like a simulator through the memory mapping file in memory</summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public virtual void OnMemorySwitchCommand(string name, int index, byte value)
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
        if (BallSearchOptions.IsSearchEnabled)
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

        if (sw.Name == _plunger_lane_switch) OnPlungerLaneActive(sw.State);

        if(_isTroughEnabled)
        {
            if (sw.Name.Contains("trough")) _trough_TroughSwitchActive(sw.Name, sw.State);
        }
        
    }

    public void PulseAutoPlunger()
    {
        if (IsPlungerSwitchActive())
        {
            Logger.Debug(nameof(MachineNode), ":auto plunging ball");
            CoilPulse(_auto_plunge_solenoid);
        }
        else { Logger.Debug(nameof(MachineNode), ": can't plunging when plunger lane is inactive."); }
    }

    /// <summary>Pulse trough if plunger switch not active</summary>
    public virtual void PulseTrough()
    {
        if (!Machine.IsSwitchOn((_plunger_lane_switch)))
            CoilPulse(_trough_solenoid);
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
    public virtual void SetSwitch(int swNum, byte value, bool fromAction = true)
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

        //sends SwitchCommandEventHandler
        EmitSignal(nameof(SwitchCommand), @switch.Name, @switch.Num, value);
    }

    /// <summary> Called when Enter Tree </summary>
    public virtual void SetupBallSearch()
    {
        //ball search options
        BallSearchOptions = new BallSearchOptions(_ball_search_coils, _ball_search_stop_switches, null, _ball_search_wait_time_secs, _ball_search_enabled);
        //create and add a ball search timer
        BallSearchTimer = new Timer() { Autostart = false, OneShot = false };
        BallSearchTimer.Connect("timeout", new Callable(this, nameof(OnBallSearchTimeout)));
        this.AddChild(BallSearchTimer);
    }

    /// <summary>
    /// Starts multi-ball trough
    /// </summary>
    /// <param name="numOfBalls">num of balls to save</param>
    /// <param name="ballSaveTime"></param>
    /// <param name="pulseTimerDelay">Timer to pulse trough</param>
    public void StartMultiball(byte numOfBalls, float pulseTimerDelay = 1)
    {
        if (pulseTimerDelay > 0)
            _startMballTrough(pulseTimerDelay);
        else
            OnMultiballStarted();
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

    /// <summary>Record a godot normal godot action, not a switch. Recording game reset.</summary>
    /// <param name="action"></param>
    /// <param name="state"></param>
    internal void RecordAction(string action, byte state)
    {
        //record the switch
        _recordingNode?.RecordEventByAction(action, state);
    }

    /// <summary>Adds custom machine items to the Machine. Actions are created for switches if they do not exist</summary>
    /// <param name="coils"></param>
    /// <param name="switches"></param>
    /// <param name="lamps"></param>
    /// <param name="leds"></param>
    protected virtual void AddCustomMachineItems(
        Dictionary<string, byte> coils,
        Dictionary<string, byte> switches,
        Dictionary<string, byte> lamps,
        Dictionary<string, byte> leds)
    {
        //override the scene setup with a machine.json
        //repopulate the collections from the json
        if (this.machinejson != null)
        {
            var json = Json.Stringify(machinejson.Data);
            MachineProcJson = JsonSerializer.Deserialize<MachineProcJson>(json);

            Logger.Info(nameof(MachineNode), ": machine.json found, populating machine from the items found.");
            
            if (MachineProcJson.PRSwitches?.Any() ?? false)
            {
                //clear and add all switches found
                switches.Clear();
                foreach (var item in MachineProcJson.PRSwitches)
                {
                    item.Num = uint.Parse(item.Number);
                    switches.Add(item.Name, (byte)item.Num);
                }

                //create new ball search options from the json
                //sets all the reset, stop for ball searching on the switch
                if (MachineProcJson.PRBallSearch != null)
                {
                    BallSearchOptions = new BallSearchOptions(
                        MachineProcJson.PRBallSearch.PulseCoils.ToArray(),
                        MachineProcJson.PRBallSearch.StopSwitches.Select(x => x.Key).ToArray(),
                        MachineProcJson.PRBallSearch.ResetSwitches.Select(x => x.Key).ToArray());
                }

                if(MachineProcJson.PRCoils?.Any() ?? false)
                {
                    coils.Clear();
                    foreach (var driver in MachineProcJson.PRCoils)
                    {
                        driver.Num = (byte)driver.NumberPROC;
                        coils.Add(driver.Name, driver.Num);
                    }
                }

                if(MachineProcJson.PRLeds?.Any() ?? false)
                {
                    leds.Clear();
                    foreach (var led in MachineProcJson.PRLeds)
                    {
                        led.Num = (byte)led.NumberPROC;
                        leds.Add(led.Name, led.Num);
                    }
                }
            }
        }

        //Set up switches / coils from the scene
        //ADD DRIVERS
        foreach (var coil in coils.Keys)
        {
            Machine.Coils.Add(coil, new PinStateObject(coils[coil]));
        }
        var itemAddResult = string.Join(", ", coils.Keys);
        Logger.Debug(nameof(MachineNode), $":added coils {itemAddResult}");
        coils.Clear();

        //ADD SWITCHES
        foreach (var sw in switches.Keys)
        {
            //create an action for the switch if it doesn't exist.
            var swVal = switches[sw];
            if (!InputMap.HasAction("sw" + swVal))
                InputMap.AddAction("sw" + swVal);

            if (BallSearchOptions.StopSearchSwitches?.Any(x => x == sw) ?? false)
                Machine.Switches.Add(sw, new Switch(sw, swVal, BallSearchSignalOption.Off));
            else if (BallSearchOptions.ResetSearchSwitches?.Any(x => x == sw) ?? false)
                Machine.Switches.Add(sw, new Switch(sw, swVal, BallSearchSignalOption.Reset));
            else
                Machine.Switches.Add(sw, new Switch(sw, swVal, BallSearchSignalOption.None));
        }

        //ADD LAMPS
        foreach (var lamp in lamps.Keys)
        {
            Machine.Lamps.Add(lamp, new PinStateObject(lamps[lamp]));
        }
        //itemAddResult = string.Join(", ", lamps.Keys);
        //LogDebug($"pingod: added lamps {itemAddResult}");
        lamps.Clear();

        //ADD LEDS
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
        Logger.Debug(nameof(MachineNode), ": ball saved");

        //only pulse early switches
        if (earlySwitch)
        {
            Logger.Debug(nameof(MachineNode), ": early ball saved, kicking ball.");
        }

        PulseTrough();

        //_pingod.OnBallSaved(((PinGodGame)_pingod).GetTree());
        _pingod.EmitSignal("BallSaved");
    }

    private void _EnterTreeSetup()
    {
        //ball search
        SetupBallSearch();

        //adds machine items and actions
        AddCustomMachineItems(_coils, _switches, _lamps, _leds);

        //memory map (windows)
        if (GetParent().HasNode("MemoryMap"))
            _pinGodMemoryMapNode = GetParent().GetNode<MemoryMapNode>("MemoryMap");

        if (HasNode(nameof(RecordingNode)))
            _recordingNode = GetNode<RecordingNode>(nameof(RecordingNode));

        if (HasNode(Paths.ROOT_PINGODGAME))
            _pingod = GetNode<IPinGodGame>(Paths.ROOT_PINGODGAME);

        //hook up to the ball saved event
        AddChild(_ballSaver);
        _ballSaver.BallSaved += _ballSaver_BallSaved;

        //setup timer to pulse trough during multiballs
        troughPulseTimer = new Timer
        {
            Name = "TroughPulseTimer",
            OneShot = false,
            WaitTime = 1
        };
        AddChild(troughPulseTimer);
        troughPulseTimer.Timeout += _trough_pulse_timeout;
        Logger.Debug(nameof(MachineNode), ":added TroughPulseTimer");
    }

    void _startMballTrough(float delay)
    {
        Logger.Debug(nameof(MachineNode), ":start m-ball trough pulse timer delay: " + delay);
        troughPulseTimer.Start(delay);
    }

    void _trough_pulse_timeout()
    {
        //stop timer if multiball stopped
        if (!_pingod?.IsMultiballRunning ?? false)
        {
            Logger.Debug(nameof(MachineNode),
                ": stopping trough pulse timeout. multiball ended or unavailable");
            troughPulseTimer.Stop();
            return;
        }

        //ball isn't in plunger lane
        if (!IsPlungerSwitchActive())
        {
            //pulse trough for ball saver
            if (_ballSaver.TimeRemaining > 0)
            {
                var bit = BallsInTrough();
                var b = _trough_switches.Length - bit;
                Logger.Debug(nameof(MachineNode),
                    ":balls in trough=" + bit + $":ball={b}:numToSave:{_ballSaver._number_of_balls_to_save}");

                if (_ballSaver._number_of_balls_to_save > 1)
                {
                    if (b < _ballSaver._number_of_balls_to_save)
                    {
                        PulseTrough();
                    }
                }
            }
        }
        else
        {
            Logger.Debug(nameof(MachineNode),
                ": plunger lane is active, can't put ball in lane while active.");
            PulseAutoPlunger();
        }
    }

    private void OnMultiballStarted() => CallDeferred(nameof(_startMballTrough), 1f);

    private void OnPlungerLaneActive(byte state)
    {
        if (state > 0)
        {
            if (!_pingod.BallStarted && _set_ball_started_on_plunger_lane)
            {
                _isNewBall = true;
                _pingod.BallStarted = true;
                Logger.Debug(nameof(MachineNode), ": ball started. is set to start on lane.");
            }
            else if (_pingod.BallStarted)
            {
                if (_ballSaver.IsBallSaveActive())
                {
                    Logger.Debug(nameof(MachineNode), ": ball saved, pulsing auto plunge");
                    PulseAutoPlunger();
                }
                else { Logger.Debug(nameof(MachineNode), ": plunger lane active"); }
            }
        }
        else
        {
            if (_isNewBall && _pingod.BallStarted)
            {
                _isNewBall = false;
                if (_set_ball_save_on_plunger_lane)
                {
                    //set ball save leave the plunger lane
                    Logger.Debug(nameof(MachineNode), ": ball save started");
                    _ballSaver.StartSaver();
                }
            }
        }
    }

    private void _trough_TroughSwitchActive(string name, byte value)
    {
        Logger.Debug(nameof(_trough_TroughSwitchActive), ":", name, "=", value);
        if (value > 0)
        {
            if (IsTroughFull())
            {
                if (_ballSaver?.IsBallSaveActive() ?? false)
                {
                    PulseTrough();
                    _ballSaver.EmitSignal("BallSaved");
                }
                else
                {
                    _pingod.EmitSignal("BallDrained");
                }
            }
        }
    }
}