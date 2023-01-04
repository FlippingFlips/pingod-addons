﻿using Godot;
using Godot.Collections;
using System.Linq;
using System.Xml.Linq;
using static PulseTimer;

/// <summary>
/// Machine Godot Node. Holds all machine items before runtime. Adds to the static collections in <see cref="Machine"/>
/// </summary>
public partial class PinGodMachine : Node
{
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

    static bool _instanceLoaded = false;
    /// <summary>
    /// How long to wait for ball searching and reset
    /// </summary>
    [Export] private int _ball_search_wait_time_secs = 10;

    [Export] Dictionary<string, byte> _coils = new Dictionary<string, byte>();
    [Export] Dictionary<string, byte> _lamps = new Dictionary<string, byte>();
    [Export] Dictionary<string, byte> _leds = new Dictionary<string, byte>();
    private PinGodMemoryMapNode _pinGodMemoryMapNode;
    [Export] Dictionary<string, byte> _switches = new Dictionary<string, byte>();
    [Signal] public delegate void CoilPulseTimedOutEventHandler(string name);

    /// <summary>
    /// Emitted when a switch comes into the game. From <see cref="PinGodMemoryMapNode.ReadStates"/> then <see cref="PinGodGame.SetSwitch(int, byte)"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    [Signal] public delegate void SwitchCommandEventHandler(string name, byte index, byte value);
    /// <summary>
    /// 
    /// </summary>
    public BallSearchOptions BallSearchOptions { get; private set; }
    /// <summary>
    /// Timer node
    /// </summary>
    public Timer BallSearchTimer { get; set; }

    /// <summary>
    /// <see cref="AddCustomMachineItems"/>
    /// </summary>
    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
			if (_instanceLoaded)
			{
                Logger.Warning(nameof(PinGodMachine), $":_EnterTree multiple {nameof(PinGodMachine)} found, removing..");
				this.QueueFree();
				return;
            }

            _instanceLoaded = true;

            //ball search options
            BallSearchOptions = new BallSearchOptions(_ball_search_coils, _ball_search_stop_switches, _ball_search_wait_time_secs, _ball_search_enabled);

            //adds machine items and actions
			AddCustomMachineItems(_coils, _switches, _lamps, _leds);

            if (GetParent().HasNode("MemoryMap"))
            {
                _pinGodMemoryMapNode = GetParent().GetNode<PinGodMemoryMapNode>("MemoryMap");
            }

            //create and add a ball search timer
            BallSearchTimer = new Timer() { Autostart = false, OneShot = false };
            BallSearchTimer.Connect("timeout", new Callable(this, nameof(OnBallSearchTimeout)));
            this.AddChild(BallSearchTimer);
        }		
	}

    public override void _Ready()
    {
        base._Ready();
        if (_pinGodMemoryMapNode != null)
        {
            //listen for events from a memory map
            _pinGodMemoryMapNode.MemorySwitchSignal += OnSwitchCommand;
            Logger.Debug(nameof(PinGodMachine), ":listening for events from plug-in ", nameof(PinGodMemoryMapNode));
        }
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
                Logger.Debug(nameof(PinGodGame), ":pulsing search coils");
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
        Logger.Info("set switch from action: " + fromAction);
        if (!fromAction)
            @switch.SetSwitch(value > 0);

        //ProcessSwitch(@switch);
        EmitSignal(nameof(SwitchCommand), @switch.Name, @switch.Num, value);
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
    /// Creates a time a connects with the 'pulsetimer_{name}'. Timeout removes the timer sets the state 0
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pulse">milliseconds. Has an integer here because in simulation, we can use higher if want longer timer</param>
    internal void CoilPulse(string name, int pulse = 255)
    {
        Logger.Info("coil pulse timer " + name);
        AddChild(new PulseTimer() { Autostart = true, OneShot = true, Name = name, WaitTime = pulse });
    }

    /// <summary>
    /// Adds custom machine items. Actions are created for switches if they do not exist
    /// </summary>
    /// <param name="coils"></param>
    /// <param name="switches"></param>
    /// <param name="lamps"></param>
    /// <param name="leds"></param>
    protected void AddCustomMachineItems(Dictionary<string, byte> coils, Dictionary<string, byte> switches, Dictionary<string, byte> lamps, Dictionary<string, byte> leds)
    {
        foreach (var coil in coils.Keys)
        {
            Machine.Coils.Add(coil, new PinStateObject(coils[coil]));
        }
        var itemAddResult = string.Join(", ", coils.Keys);
        Logger.Debug(nameof(PinGodMachine), $":added coils {itemAddResult}");
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

        Logger.Debug(nameof(PinGodMachine), $":Custom items loaded...");
        Logger.Debug(nameof(PinGodMachine), $":switches={Machine.Switches.Count}:coils={Machine.Coils.Count}:lamps={Machine.Lamps.Count},:leds={Machine.Leds.Count}");
    }

    private void OnSwitchCommand(string name, int index, byte value)
    {
        Logger.Debug(nameof(PinGodMachine), $": setSwitch--n:{name},numVal:{index}-{value}");
        if (string.IsNullOrWhiteSpace(name))
        {
            SetSwitch(index, value, false);
        }
        else
        {
            SetSwitch(name, value, false);
        }
        
    }

}