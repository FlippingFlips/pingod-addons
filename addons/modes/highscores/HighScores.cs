using Godot;
using PinGod.Base;
using PinGod.Core;
using System.Linq;

namespace PinGod.Modes
{
    /// <summary>
    /// Display high scores from the saved <see cref="Audits.HighScores"/>
    /// </summary>
    public partial class HighScores : Control
    {
        internal IPinGodGame pinGod;
        private Label Label;

        /// <summary>
        /// Gets objects from scene
        /// </summary>
        public override void _EnterTree()
        {
            if (HasNode("/root/PinGodGame"))
            {
                pinGod = GetNode("/root/PinGodGame") as IPinGodGame;
            }
            Label = GetNode("CenterContainer/VBoxContainer/Label") as Label;
        }

        /// <summary>
        /// Gets the top high scores from <see cref="pinGod"/> <see cref="Audits.HighScores"/> and sets the label with scores
        /// </summary>
        public override void _Ready()
        {
            if (pinGod?.Audits?.HighScores != null)
            {
                var scores = string.Join("\n", pinGod?.Audits?.HighScores?.Select(x => $"{x.Scores.ToScoreString()}    {x.Name}"));
                Label.Text = scores;
            }
            else
            {
                Label.Text = $"2,000,000    DUB\n1,000,000    D I\n500,000    TER";
            }
        }
    }
}
