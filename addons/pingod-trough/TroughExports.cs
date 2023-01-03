using Godot;

/// <summary>
/// Trough Exports for Godot editor
/// </summary>
public partial class Trough : Node
{
    /// <summary>
    /// The solenoid / coil name to use when kicking the ball in the plunger lane
    /// </summary>
    [Export] public string _auto_plunge_solenoid = "auto_plunger";

    /// <summary>
    /// The lamp name to cycle for ball saves
    /// </summary>
    [Export] public string _ball_save_lamp = "";

    /// <summary>
    /// The led name to cycle for ball saves
    /// </summary>
    [Export] public string _ball_save_led = "shoot_again";

    /// <summary>
    /// default ball save in multi-ball
    /// </summary>
    [Export] public byte _ball_save_multiball_seconds = 8;

    /// <summary>
    /// default ball save time
    /// </summary>
    [Export] public byte _ball_save_seconds = 8;

    /// <summary>
    /// Early switch ball save names. outlane_l outlane_r
    /// </summary>
    [Export] public string[] _early_save_switches = { "outlane_l", "outlane_r" };

    /// <summary>
    /// Use this to turn off trough checking, outside VP
    /// </summary>
    [Export] public bool _isDebugTrough = false;

    /// <summary>
    /// number of balls to save, defaults to one single ball play
    /// </summary>
    [Export] public byte _number_of_balls_to_save = 1;

    /// <summary>
    /// THe switch name used for the plunger lane
    /// </summary>
    [Export] public string _plunger_lane_switch = "plunger_lane";

    [Export] public bool _set_ball_save_on_plunger_lane = true;

    /// <summary>
    /// Sets the <see cref="PinGodGame.BallStarted"/>
    /// </summary>
    [Export] public bool _set_ball_started_on_plunger_lane = true;

    /// <summary>
    /// The solenoid / coil name to use when kicking the ball from the trough
    /// </summary>
    [Export] public string _trough_solenoid = "trough";

    /// <summary>
    /// Switch names, defaults trough_1 -- trough_4
    /// </summary>
    [Export] public string[] _trough_switches = { "trough_1", "trough_2", "trough_3", "trough_4" };
}
