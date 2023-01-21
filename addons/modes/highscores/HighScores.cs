using Godot;
using PinGod.Base;
using System.Collections.Generic;

namespace PinGod.Modes
{
    /// <summary>
    /// Display high scores using BBCode table. This is an attempt to loose couple the scenes <para/>
    /// Use GetNode("%HighScores")?.Call("AddHighScore", score.Name, score.Points); to add scores to this local list, then UpdateScoresText
    /// </summary>
    [Tool]
    public partial class HighScores : Control
    {
        List<HighScore> _scores;

        public void SetScoresText(string text) => GetNode<RichTextLabel>("%ScoresTableLabel").Text = text;

        public override void _Ready()
        {
            base._Ready();
            _scores = new List<HighScore>();

            if (Engine.IsEditorHint())
            {
                _scores.Add(new HighScore { Name = "DUB", Points = 200000 });
                _scores.Add(new HighScore { Name = "CAN", Points = 100000 });
                UpdateScoresText();
            }
        }

        void AddHighScore(string name, long points) => _scores.Add(new HighScore { Name = name, Points = points });

        /// <summary>
        /// https://docs.godotengine.org/en/latest/tutorials/ui/bbcode_in_richtextlabel.html#doc-bbcode-in-richtextlabel-cell-options
        /// </summary>
        /// <param name="highScores"></param>
        public void UpdateScoresText()
        {
            string scores = "[table=2]\n";
            foreach (var score in _scores)
            {
                scores += $"[cell=10]{score.Name}\t[/cell]";
                scores += $"[cell=500]{score.Points.ToScoreString()}[/cell]";
            }
            scores += "[/table]";

            SetScoresText(scores);
        }
    }
}
