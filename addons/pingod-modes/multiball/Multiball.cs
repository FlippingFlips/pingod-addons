using Godot;
using PinGod.Core;
using PinGod.Core.addons.ModeTimer;

namespace PinGod.Modes
{
    /// <summary>
    /// Starts multi-ball using pinGod and removes this scene when EndMultiball is called. <para/>
    /// Scenes that are in a group named multiball, like this scene file default is, will have EndMultiball invoked <see cref="EndMultiball"/> <para/>
    /// <see cref="Game.EndMultiball"/>
    /// </summary>
    public partial class Multiball : Control
    {
        private IPinGodGame pinGod;

        /// <summary>
        /// 4 ball multi-ball
        /// </summary>
        [Export] byte _num_of_balls = 4;

        [Export] byte _ball_save_time_seconds = 12;

        [Export] bool _showModeTimer = true;

        /// <summary>
        /// Gets a reference for <see cref="pinGod"/>
        /// </summary>
        public override void _EnterTree()
        {
            pinGod = GetNode(Paths.ROOT_PINGODGAME) as IPinGodGame;
            if (pinGod == null)
            {
                Logger.Info(nameof(Multiball), " this module requires " + nameof(IPinGodGame), " in the root, exiting.");
                this.QueueFree();
            }
        }

        /// <summary>
        /// Starts multi-ball <see cref="PinGodGame.StartMultiBall(byte, byte, float)"/>
        /// </summary>
        public override void _Ready()
        {
            Logger.Debug(nameof(Multiball), ": _ready: secs/balls", _ball_save_time_seconds, _num_of_balls);

            //hide timer
            if (!_showModeTimer)
            {
                var timer = GetNode("ModeTimer") as ModeTimer;
                timer.IsVisible(false);
            }

            pinGod.StartMultiBall(_num_of_balls, _ball_save_time_seconds);
        }

        /// <summary>
        /// Removes this control from the tree (Signal is emitted from Trough)
        /// </summary>
        public virtual void EndMultiball()
        {
            Logger.Debug(nameof(Multiball), ":", nameof(EndMultiball));
            this.QueueFree();
        }
    }
}