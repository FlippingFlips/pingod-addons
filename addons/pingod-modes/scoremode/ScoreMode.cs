using Godot;
using Godot.Collections;
using PinGod.Base;
using PinGod.Core;
using PinGod.Core.Game;

namespace PinGod.Modes
{
    /// <summary>
    /// Simple score display mode for 4 players with ball information. Used in the <see cref="Game"/> Scene
    /// </summary>
    public partial class ScoreMode : Node
    {
        /// <summary>
        /// 
        /// </summary>
        protected IPinGodGame pinGod;

        [ExportCategory("debug")]
        [Export] bool _isDebugMode = false;
        [Export] protected bool _show_main_score_multiplayer = true;

        /// <summary>
        /// Show Player ones ScoreP1 label if set to true. Normally in a pinball the scorep1 would not display with main score unless multi-player
        /// </summary>
        [ExportCategory("options")]
        [Export] protected bool _single_player_p1_visible = false;

        #region Node paths to select in scene
        [ExportCategory("score labels")]
        [Export] NodePath _ballInfoLabel = null;
        [Export] NodePath _playerInfoLabel = null;
        [Export] NodePath _scoreLabel = null;
        [Export] Array<NodePath> _scoreLabels = null;
        #endregion

        #region Labels from Scene
        /// <summary>
        /// 
        /// </summary>
        protected Label ballInfolabel;
        /// <summary>
        /// Player information label
        /// </summary>
        protected Label playerInfoLabel;

        /// <summary>
        /// Main score label
        /// </summary>
        protected Label scoreLabel;
        /// <summary>
        /// Other player scoreLabels.
        /// </summary>
        protected Label[] ScoreLabels;
        #endregion

        public PinGodPlayer[] Players { get; private set; }

        /// <summary>
        /// Connects to signals to update scores
        /// </summary>
        public override void _EnterTree()
        {
            if (HasNode(Paths.ROOT_PINGODGAME))
            {
                pinGod = GetNode(Paths.ROOT_PINGODGAME) as IPinGodGame;
                //signals
                pinGod.Connect(nameof(PinGodBase.GameStarted),
                    new Callable(this, nameof(OnScoresUpdated)));

                pinGod.Connect(nameof(PinGodBase.ScoresUpdated),
                    new Callable(this, nameof(OnScoresUpdated)));

                pinGod.Connect(nameof(PinGodBase.PlayerAdded),
                    new Callable(this, nameof(OnScoresUpdated)));

                (pinGod as PinGodBase).CreditAdded += UpdateCredits;
                UpdateCredits(pinGod?.Audits?.Credits ?? 0);
            }
            else
            {
                Logger.Debug(nameof(ScoreMode), $": no PinGodGame could be found");
                if (_isDebugMode)
                {
                    SetupDebugMode();
                }
            }
        }        

        /// <summary>
        /// Retrieves all labels in the scene. Checks to see if we have <see cref="_scoreLabels"/> set. <para/>
        /// Will call <see cref="OnScoresUpdated"/>
        /// </summary>
        public override void _Ready()
        {
            if (_scoreLabels?.Count <= 0) Logger.Warning("No _scoreLabels have been defined for the ScoreMode.");
            else ScoreLabels = new Label[_scoreLabels.Count];

            GetBallPlayerInfoLabels();
            GetPlayerScoreLabels();
            CallDeferred(nameof(OnScoresUpdated));
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            (pinGod as PinGodBase).CreditAdded -= UpdateCredits;
        }

        /// <summary>
        /// Assigns the given node paths to Labels. Ball, Player, Main Score
        /// </summary>
        public virtual void GetBallPlayerInfoLabels()
        {
            ballInfolabel = _ballInfoLabel == null ? null : GetNode<Label>(_ballInfoLabel);
            playerInfoLabel = _playerInfoLabel == null ? null : GetNode<Label>(_playerInfoLabel);
            scoreLabel = _scoreLabel == null ? null : GetNode<Label>(_scoreLabel);
        }

        /// <summary>
        /// Assigns the player score labels and resets the initial text of the label. 
        /// </summary>
        public virtual void GetPlayerScoreLabels()
        {
            for (int i = 0; i < _scoreLabels?.Count; i++)
            {
                var node = GetNode(_scoreLabels[i]) as Label;
                ScoreLabels[i] = node;
                if (node != null)
                    ScoreLabels[i].Text = null;
            }
        }

        /// <summary>
        /// override this but this will invoke <see cref="OnScoresUpdated"/> to update scene labels
        /// </summary>
        public virtual void OnBallStarted() => OnScoresUpdated();

        /// <summary>
        /// Updates all the labels text. Override <see cref="UpdateMainScore"/>, <see cref="UpdatePlayerScores"/>, <see cref="UpdatePlayerBallInfo"/>
        /// </summary>
        public virtual void OnScoresUpdated()
        {
            if (!_isDebugMode)
            {
                UpdateMainScore();
                UpdatePlayerScores();
                UpdatePlayerBallInfo();
            }
            else
            {
                //main score
                if (Players.Length > 1 && !_show_main_score_multiplayer) scoreLabel.Text = null;
                else scoreLabel.Text = (Players[0].Points).ToScoreString();
                //all players
                for (int i = 0; i < ScoreLabels.Length - 1; i++) { ScoreLabels[i].Text = (Players[i].Points).ToScoreString(); }
                //ball info
                if (ballInfolabel != null) ballInfolabel.Text = Tr("BALL") + " 3";
                //player info
                if (playerInfoLabel != null) playerInfoLabel.Text = Tr("PLAYER") + " PLAYER: 1";
            }
        }

        /// <summary>
        /// in all player labels update their scores
        /// </summary>
        public virtual void UpdatePlayerScores()
        {
            if (pinGod?.Players?.Count <= 0) return;

            int i = 0;
            foreach (var player in pinGod.Players)
            {
                //this hides displaying a multi-player score for the single player.
                if (pinGod.Players.Count == 1 && i == 0 && !_single_player_p1_visible)
                {
                    i++;
                    continue;
                }
                var lbl = ScoreLabels[i];
                if (lbl != null)
                {
                    if (player.Points > -1)
                    {
                        lbl.Text = player.Points.ToScoreString();
                    }
                    else
                    {
                        lbl.Text = null;
                    }
                }
                i++;
            }
        }

        public virtual void UpdateMainScore()
        {
            if (pinGod?.Players?.Count <= 0) return;

            if (scoreLabel != null)
            {
                if (pinGod.Player.Points > -1)
                {
                    //more than 1 player, show main score?
                    if (pinGod.Players.Count > 1 && !_show_main_score_multiplayer)
                    {
                        scoreLabel.Text = null;
                    }
                    //set main score label
                    else { scoreLabel.Text = pinGod.Player.Points.ToScoreString(); }
                }
                else
                {
                    scoreLabel.Text = null;
                }
            }
        }

        /// <summary>
        /// Updates the current player and ball from PinGod BallInPlay and CurrentPlayerIndex
        /// </summary>
        public virtual void UpdatePlayerBallInfo()
        {
            if (pinGod?.Players?.Count <= 0) return;

            if (ballInfolabel != null) 
                ballInfolabel.Text = Tr("BALL") + " " + pinGod.BallInPlay.ToString();
            if (playerInfoLabel != null) 
                playerInfoLabel.Text = $"{Tr("PLAYER")}: {pinGod.CurrentPlayerIndex + 1}";

            GetNode<Label>("%CreditsLabel").Text = $"CREDITS: {pinGod?.Audits?.Credits}";
        }

        private void SetupDebugMode()
        {
            Players = new PinGodPlayer[] { new(), new(), new(), new() };
            Logger.Debug(nameof(ScoreMode), $": DEBUG. Fake players");
            Players[0].Points = 280000;
            Players[1].Points = 260000;
            Players[2].Points = 240000;
            Players[3].Points = 220000;
        }

        void UpdateCredits(int credits) => GetNode<Label>("%CreditsLabel").Text = $"CREDITS: {credits}";
    }
}
