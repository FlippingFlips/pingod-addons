using Godot;

namespace PinGod.Core.Nodes.PlungerLane
{
    public partial class PlungerLane
    {
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
    }
}