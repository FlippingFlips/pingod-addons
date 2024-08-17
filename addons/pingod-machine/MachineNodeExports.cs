using Godot;
using Godot.Collections;

public partial class MachineNode : Node
{
    [ExportCategory("Ball Search")]
    /// <summary>Coil names to pulse when ball searching</summary>
    [Export] public string[] _ball_search_coils;
    /// <summary></summary>
    [Export] public bool _ball_search_enabled = true;
    /// <summary> Switches that stop the ball searching</summary>
    [Export] public string[] _ball_search_stop_switches;
    /// <summary>How long to wait for ball searching and reset</summary>
    [Export] private int _ball_search_wait_time_secs = 10;

    [ExportCategory("Machine Items")]
    [Export] protected Dictionary<string, byte> _coils = new();
    [Export] protected Dictionary<string, byte> _lamps = new();
    [Export] protected Dictionary<string, byte> _leds = new();
    [Export] protected Dictionary<string, byte> _switches = new();

    [ExportCategory("Trough")]
    /// <summary>
    /// Use this to turn off trough checking, outside VP
    /// </summary>
    [Export] public bool _isDebugTrough = false;

    [Export] public bool _isTroughEnabled = true;

    /// <summary>
    /// The solenoid / coil name to use when kicking the ball from the trough
    /// </summary>
    [Export] public string _trough_solenoid = "trough";

    /// <summary>
    /// Switch names, defaults trough_1 -- trough_4
    /// </summary>
    [Export] public string[] _trough_switches = { "trough0", "trough1", "trough2", "trough3" };

    [ExportCategory("PlungerLane")]
    /// <summary>
    /// The solenoid / coil name to use when kicking the ball in the plunger lane
    /// </summary>
    [Export] public string _auto_plunge_solenoid = "auto_plunger";

    /// <summary>
    /// THe switch name used for the plunger lane
    /// </summary>
    [Export] public string _plunger_lane_switch = "plungerLane";

    /// <summary>
    /// Sets the <see cref="PinGodGame.BallStarted"/>
    /// </summary>
    [Export] public bool _set_ball_started_on_plunger_lane = true;

    [Export] public bool _set_ball_save_on_plunger_lane = true;

    [Export] public bool _isEnabled = true;

}
