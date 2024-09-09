using Godot;

namespace PinGod.Modes
{
    /// <summary>
    /// Simple scene to show blinking ball save and disposes (queue free) when the timer ends
    /// </summary>
    public partial class BallSave : Control
    {
        [Export] float _remove_after_time = 2f;

        private Label _debugLabel;

        /// <summary>
        /// Finds a Timer named Timer and sets the wait time to <see cref="_remove_after_time"/>
        /// </summary>
        public override void _EnterTree()
        {
            base._EnterTree();
            GetNode<Timer>("Timer").WaitTime = _remove_after_time;

            _debugLabel = GetNodeOrNull<Label>("CenterContainer/VBoxContainer/DebugLabel");
        }
        /// <summary>
        /// Resets . sets the <see cref="_remove_after_time"/>
        /// </summary>
        /// <param name="time"></param>
        public void SetRemoveAfterTime(float time) => _remove_after_time = time;

        void _on_Timer_timeout() => this.QueueFree();

        /// <summary> Show optional debug info </summary>
        /// <param name="text"></param>
        public void SetDebugLabelText(string text)
        {
            if(_debugLabel != null)
            {
                _debugLabel.Text = text;
                _debugLabel.Visible = true;
            }            
        }
    }
}
