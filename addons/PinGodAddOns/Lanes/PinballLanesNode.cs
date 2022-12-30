﻿using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Set the exports in the scene from Godot or tscn. <see cref="_lane_switches"/> <see cref="_lane_lamps"/> and other options.
/// </summary>
public class PinballLanesNode : PinGodGameMode
{
    bool[] _lanesCompleted;
    private byte[] _laneSwitchNums;

    #region Exports
    [Export] bool _flipper_changes_lanes = true;
    /// <summary>
    /// Lamps to update
    /// </summary>
    [Export] string[] _lane_lamps = new string[0];
    /// <summary>
    /// Leds to update
    /// </summary>
    [Export] string[] _led_lamps = new string[0];
    /// <summary>
    /// Switches to handle lane events
    /// </summary>
    [Export] string[] _lane_switches = new string[0];
    /// <summary>
    /// Default color to use for led. Color8, RGB
    /// </summary>

    [Export] protected Color _led_color = Color.Color8(255, 255, 255);

    [Export] bool _resetOnLanesCompleted = true;

    [Export] bool _resetOnBallStarted = true;

    #endregion

    #region Signals
    /// <summary>
    /// Emitted in <see cref="CheckLanes"/> if complete
    /// </summary>
    [Signal] public delegate void LanesCompleted();

    /// <summary>
    /// Emitted in <see cref="LaneSwitchActivated"/> if complete
    /// </summary>
    [Signal] public delegate void LaneCompleted(string swName, bool complete);
    #endregion

    /// <summary>
    /// Will Free this mode if no switches are set
    /// </summary>
    public override void _EnterTree()
    {
        if (!Engine.EditorHint)
        {
            base._EnterTree();

            if (_lane_switches == null)
            {
                Logger.Error("no rollover switches defined. removing mode");
                this.QueueFree();
            }
            else
            {
                _lanesCompleted = new bool[_lane_switches.Length];
                _laneSwitchNums = new byte[_lane_switches.Length];
                for (int i = 0; i < _lane_switches.Length; i++)
                {
                    _laneSwitchNums[i]= Machine.Switches[_lane_switches[i]].Num;
                }
                
                pinGod.Connect(nameof(PinGodBase.SwitchCommand), this, nameof(OnSwitchCommandHandler));
            }
        }
    }

    private void OnSwitchCommandHandler(string name, byte index, byte value)
    {        
        if(_lane_switches?.Length > 0)
        {
            if(value > 0)
            {
                switch (name)
                {
                    case "flipper_l":
                        if (_flipper_changes_lanes)
                            RotateLanesLeft();
                        return;
                    case "flipper_r":
                        if (_flipper_changes_lanes)
                            RotateLanesRight();
                        return;
                    default:
                        break;
                }

                bool wasSet = false;
                for (int i = 0; i < _laneSwitchNums.Length; i++)
                {
                    if (_laneSwitchNums[i] == index)
                    {
                        wasSet = LaneSwitchActivated(i);
                        CheckLanes();
                        UpdateLamps();
                        break;
                    }                    
                }
            }
            else //switch off
            {

            }
        }        
    }

    /// <summary>
    /// Handles flipper inputs and changes lanes. <see cref="RotateLanesLeft"/> and <see cref="RotateLanesRight"/>. <para/>
    /// Checks if any of the <see cref="_lane_switches"/> were used and sets <see cref="LaneSwitchActivated(int)"/> <para/>
    /// Checks if lanes completed and updates lamps
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        if (Engine.EditorHint)
        {
            SetProcessInput(false);
            return;
        }

        if (_lane_switches != null)
        {
            //just to handle window actions
            for (int i = 0; i < _lane_switches.Length; i++)
            {
                pinGod.SwitchActionOn(_lane_switches[i], @event);
            }
        }
    }

    /// <summary>
    /// Checks <see cref="_lanesCompleted"/>
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckLanes()
    {
        bool complete = true;
        for (int i = 0; i < _lanesCompleted.Length; i++)
        {
            if (!_lanesCompleted[i])
            {
                complete = false;
                break;
            }
        }
        if (complete)
        {
            EmitSignal(nameof(LanesCompleted));
            if (_resetOnLanesCompleted) ResetLanesCompleted();
        }
        return complete;
    }

    /// <summary>
    /// Get all Completed Lanes. <see cref="_lanesCompleted"/>
    /// </summary>
    /// <returns></returns>
    public virtual bool[] GetCompletedLanes() => _lanesCompleted;

    /// <summary>
    /// Returns true if the lane was completed after this lane was activated
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public virtual bool LaneSwitchActivated(int i)
    {
        bool result = false;
        if (!_lanesCompleted[i])
        {
            _lanesCompleted[i] = true;
            Logger.Debug(nameof(PinballLanesNode), $":lane {i} complete");

            result = true;
        }

        EmitSignal(nameof(LaneCompleted), _lane_switches[i], result);
        return result;
    }

    /// <summary>
    /// Resets the lanes complete if <see cref="_resetOnBallStarted"/>
    /// </summary>
    protected override void OnBallStarted()
    {
        base.OnBallStarted();
        if (_resetOnBallStarted)
            ResetLanesCompleted();
    }

    /// <summary>
    /// Resets <see cref="_lanesCompleted"/>
    /// </summary>
    public virtual void ResetLanesCompleted() => _lanesCompleted = new bool[_lane_switches.Length];

    /// <summary>
    /// Cycles the lane targets, lamps to the left
    /// </summary>
    public virtual void RotateLanesLeft()
    {
        var firstNum = _lanesCompleted[0];
        for (int i = 0; i < _lanesCompleted.Length - 1; i++)
        {
            _lanesCompleted[i] = _lanesCompleted[i + 1];
        }
        _lanesCompleted[_lanesCompleted.Length - 1] = firstNum;

        UpdateLamps();
        Logger.Debug(nameof(PinballLanesNode),":rot left: ", string.Join(",", _lanesCompleted));
    }

    /// <summary>
    /// Cycles the lane targets, lamps to the right
    /// </summary>
    public virtual void RotateLanesRight()
    {
        var lastNum = _lanesCompleted[_lanesCompleted.Length - 1];
        for (int i = _lanesCompleted.Length - 1; i > 0; i--)
        {
            _lanesCompleted[i] = _lanesCompleted[i - 1];
        }
        _lanesCompleted[0] = lastNum;

        UpdateLamps();
        Logger.Debug(nameof(PinballLanesNode), ":rot right: ", string.Join(",", _lanesCompleted));
    }

    /// <summary>
    /// Updates LEDS or LAMPS and sets their state
    /// </summary>
    protected override void UpdateLamps()
    {
        base.UpdateLamps();

        if (_lanesCompleted != null)
        {
            if (_lane_lamps?.Length > 0)
            {
                for (int i = 0; i < _lanesCompleted.Length; i++)
                {
                    if (_lanesCompleted[i])
                    {
                        pinGod.SetLampState(_lane_lamps[i], 1);
                    }
                    else
                    {
                        pinGod.SetLampState(_lane_lamps[i], 0);
                    }
                }
            }

            if (_led_lamps?.Length > 0)
            {
                for (int i = 0; i < _lanesCompleted.Length; i++)
                {
                    if (_lanesCompleted[i])
                    {
                        pinGod.SetLedState(_led_lamps[i], 1, _led_color);
                    }
                    else
                    {
                        pinGod.SetLedState(_led_lamps[i], 0, _led_color);
                    }
                }
            }
        }
    }
}