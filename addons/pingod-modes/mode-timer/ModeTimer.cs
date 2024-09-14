using Godot;

namespace PinGod.Core.addons.ModeTimer
{
    /// <summary>A scene timer to display time left. <para/>
    /// Set mode name and mode title in scene file and initial time</summary>
    public partial class ModeTimer : Timer
    {
        private Label titleLabel;
        private Label nameLabel;
        private Label timeLeftLabel;

        [Export] int _ModeTime = 30;
        [Export] string _ModeName = "ModeName";
        [Export] string _ModeTitle = "Mode Title";
        [Export] bool _isVisible = true;

        /// <summary>Emitted signal when a mode times out</summary>
        [Signal] public delegate void ModeTimedOutEventHandler(string title);

        /// <summary>
        /// Sets up the labels
        /// </summary>
        public override void _EnterTree()
        {
            titleLabel = GetNode("VBoxContainer/Title") as Label;
            nameLabel = GetNode("VBoxContainer/Name") as Label;
            timeLeftLabel = GetNode("VBoxContainer/TimeLeftLabel") as Label;
            titleLabel.Text = _ModeTitle;
            nameLabel.Text = _ModeName;
            timeLeftLabel.Text = string.Empty;
        }

        public bool IsVisible(bool visible)
        {
            _isVisible = visible;

            return visible;
        }

        /// <summary>adds on seconds</summary>
        /// <param name="secs"></param>
        public void IncrementTime(int secs) => _ModeTime += secs;
        public void UpdateName(string name) => nameLabel.Text = name;

        /// <summary>sets the time</summary>
        /// <param name="secs"></param>
        public void UpdateTime(int secs) => _ModeTime = secs;

        public void UpdateTitle(string title) => titleLabel.Text = title;

        /// <summary>Updates time left text.<para/>
        /// When time runs out a ModeTimedOut signal with the mode name is emitted</summary>
        private void _on_ModeTimer_timeout()
        {
            titleLabel.Visible = _isVisible;
            nameLabel.Visible = _isVisible;
            timeLeftLabel.Visible = _isVisible;
            timeLeftLabel.Text = _ModeTime.ToString();
            _ModeTime--;
            if (_ModeTime <= 0)
            {
                this.Stop();
                Logger.Debug(nameof(ModeTimer), $": {_ModeName}-{_ModeTitle} mode timed out");
                EmitSignal(nameof(ModeTimedOut), _ModeName);
            }
        }
    }
}
