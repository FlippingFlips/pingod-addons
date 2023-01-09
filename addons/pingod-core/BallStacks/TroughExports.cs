using Godot;

namespace PinGod.Core.BallStacks
{
    /// <summary>
    /// Trough Exports for Godot editor
    /// </summary>
    public partial class Trough : Node
    {
        /// <summary>
        /// Use this to turn off trough checking, outside VP
        /// </summary>
        [Export] public bool _isDebugTrough = false;

        [Export] public bool _isEnabled = true;

        /// <summary>
        /// The solenoid / coil name to use when kicking the ball from the trough
        /// </summary>
        [Export] public string _trough_solenoid = "trough";

        /// <summary>
        /// Switch names, defaults trough_1 -- trough_4
        /// </summary>
        [Export] public string[] _trough_switches = { "trough_1", "trough_2", "trough_3", "trough_4" };
    }
}
