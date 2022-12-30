﻿using Godot;

/// <summary>
/// Represents a Switch in a pinball machine
/// </summary>
public class Switch
{
    /// <summary>
    /// Initialize with number only
    /// </summary>
    /// <param name="num"></param>
    public Switch(byte num) { this.Num = num; }
    /// <summary>
    /// Initialize with number and options for ball searching
    /// </summary>
    /// <param name="num"></param>
    /// <param name="ballSearch"></param>
    public Switch(byte num, BallSearchSignalOption ballSearch) { this.Num = num; this.BallSearch = ballSearch; }

    /// <summary>
    /// Initialize with name and number with options for ball searching
    /// </summary>
    /// <param name="name"></param>
    /// <param name="num"></param>
    /// <param name="ballSearch"></param>
    public Switch(string name, byte num, BallSearchSignalOption ballSearch) { this.Name = name; this.Num = num; this.BallSearch = ballSearch; }

    /// <summary>
    /// Initialize Switch name + num
    /// </summary>
    /// <param name="name"></param>
    /// <param name="num"></param>
    public Switch(string name, byte num) { this.Name = name; this.Num = num; }
    /// <summary>
    /// Initialize
    /// </summary>
    public Switch()
    {
        Time = OS.GetSystemTimeMsecs();
    }
    /// <summary>
    /// Name of the switch
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Number of the switch
    /// </summary>
    public byte Num { get; set; }
    /// <summary>
    /// Ball search options
    /// </summary>
    public BallSearchSignalOption BallSearch { get; set; }
    /// <summary>
    /// Flag set by actions and SetSwitch
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Time last active
    /// </summary>
    public ulong Time { get; set; }

    /// <summary>
    /// Sets a switch manually, pushes a InputEventAction to Input
    /// </summary>
    /// <param name="pressed"></param>
    /// <returns></returns>
    public void SetSwitchAction(bool pressed) 
    {
        Input.ParseInputEvent(new InputEventAction() { Action = this.ToString(), Pressed = pressed });
        IsEnabled = pressed;
    }

    /// <summary>
    /// Also sets the time of the switch
    /// </summary>
    /// <param name="enabled"></param>
    public void SetSwitch(bool enabled)
    {
        IsEnabled = enabled;
        Time = OS.GetSystemTimeMsecs();
        Logger.Verbose(nameof(Switch), $":{this.Name}:{this.Num} = {IsEnabled}");
    }

    /// <summary>
    /// Checks the current input event. IsActionPressed(sw+num)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool IsActionOn(InputEvent input) 
    { 
        bool active = input.IsActionPressed(ToString());
        if (active)
        {
            SetSwitch(true);
        }
        return active;
    }
    /// <summary>
    /// Checks the current input event. IsActionReleased(sw+num)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool IsActionOff(InputEvent input) {
        bool released = input.IsActionReleased(ToString());
        if (released)
        {
            SetSwitch(false);
        }
        return released;
    }
    /// <summary>
    /// Checks if On/Off - Action pressed sw{num}
    /// </summary>
    /// <returns></returns>
    public bool IsActionOn()
    {
        IsEnabled = Input.IsActionPressed(ToString());
        return IsEnabled;
    }

    /// <summary>
    /// Time in milliseconds since switch used
    /// </summary>
    /// <returns></returns>
    public ulong TimeSinceChange()
    {
        if(Time > 0)
        {
            return OS.GetSystemTimeMsecs() - Time;
        }

        return 0;
    }
    /// <summary>
    /// The action. swNum
    /// </summary>
    /// <returns></returns>
    public override string ToString() => "sw" + Num;
}