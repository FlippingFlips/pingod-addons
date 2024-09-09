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

            //get the mode timer from the scene
            var timer = GetNode("ModeTimer") as ModeTimer;

            //hide the timer display ?
            if (!_showModeTimer)
                timer.IsVisible(false);

            //run the multi-ball, kick balls every 2 secs
            pinGod.StartMultiBall(_num_of_balls, _ball_save_time_seconds, 2);

            //connect to the mode timer for when it times out
            //we can end the multiball
            timer.Connect(
                nameof(ModeTimer.ModeTimedOut),
                Callable.From<string>(EndMultiball));
        }

        /// <summary>Removes this control from the tree<para/>
        /// The mode time when complete will invoke this callback</summary>
        public virtual void EndMultiball(string g)
        {
            Logger.Debug(nameof(Multiball), ":", nameof(EndMultiball));
            this.QueueFree();
        }
    }
}