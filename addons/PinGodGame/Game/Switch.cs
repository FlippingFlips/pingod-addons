using Godot;

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
    /// Time last active
    /// </summary>
    public ulong Time { get; set; }

    /// <summary>
    /// Sets a switch manually, pushes an InputEventAction to the queue
    /// </summary>
    /// <param name="Pressed"></param>
    /// <returns></returns>
    public void SetSwitch(bool Pressed) => Input.ParseInputEvent(new InputEventAction() { Action = this.ToString(), Pressed = Pressed });

    /// <summary>
    /// Checks the current input event. IsActionPressed(sw+num)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool IsOn(InputEvent input) 
    { 
        bool active = input.IsActionPressed(ToString());
        if (active)
        {
            Time = OS.GetSystemTimeMsecs();
            Logger.Debug(nameof(Switch), $":{this.Name}:{this.Num} on");
        }
        return active;
    }
    /// <summary>
    /// Checks the current input event. IsActionReleased(sw+num)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool IsOff(InputEvent input) {
        bool released = input.IsActionReleased(ToString());
        if (released)
        {
            Time = OS.GetSystemTimeMsecs();
            Logger.Debug(nameof(Switch), $":{this.Name}:{this.Num} off");
        }
        return released;
    }
    /// <summary>
    /// Checks if On/Off
    /// </summary>
    /// <returns></returns>
    public bool IsOn() => Input.IsActionPressed(ToString());

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
