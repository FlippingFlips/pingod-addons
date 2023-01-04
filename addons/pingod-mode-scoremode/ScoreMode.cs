using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Simple score display mode for 4 players with ball information. Used in the <see cref="Game"/> Scene
/// </summary>
public partial class ScoreMode : Node
{
    [ExportCategory("debug")]
    [Export] bool _isDebugMode = false;

    /// <summary>
    /// Show Player ones ScoreP1 label if set to true. Normally in a pinball the scorep1 would not display with main score unless multi-player
    /// </summary>
    [ExportCategory("options")]
    [Export] bool _single_player_p1_visible = false;

    [Export] bool _show_main_score_multiplayer = true;

    #region Node paths to select in scene
    [ExportCategory("score labels")]
    [Export] NodePath _ballInfoLabel = null;
    [Export] NodePath _playerInfoLabel = null;
    [Export] NodePath _scoreLabel = null;
    [Export] Array<NodePath> _scoreLabels = null;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    protected PinGodGame pinGod;

    #region Labels from Scene
    /// <summary>
    /// 
    /// </summary>
    protected Label ballInfolabel;
    /// <summary>
    /// Main score label
    /// </summary>
    protected Label scoreLabel;
    /// <summary>
    /// Player information label
    /// </summary>
    protected Label playerInfoLabel;
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
        if (HasNode("/root/PinGodGame"))
        {
            pinGod = GetNode("/root/PinGodGame") as PinGodGame;
            //signals
            pinGod.Connect("GameStarted", new Callable(this, nameof(OnScoresUpdated)));
            pinGod.Connect("ScoresUpdated", new Callable(this, nameof(OnScoresUpdated)));
            pinGod.Connect("PlayerAdded", new Callable(this, nameof(OnScoresUpdated)));
        }
        else
        { 
            Logger.Debug(nameof(ScoreMode), $": no PinGodGame could be found");
            if (_isDebugMode)
            {
                SetupDebugMode();
            }
        }

        if (_scoreLabels?.Count <= 0) Logger.Warning("No _scoreLabels have been defined for the ScoreMode.");
        else ScoreLabels = new Label[_scoreLabels.Count];
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

    /// <summary>
    /// Update current player and ball
    /// </summary>
    public override void _Ready()
    {
        GetBallPlayerInfoLabels();
        GetPlayerScoreLabels();
        CallDeferred(nameof(OnScoresUpdated));
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
        for (int i = 0; i < _scoreLabels.Count; i++)
        {
            ScoreLabels[i] = _scoreLabels[i] != null ? GetNode<Label>(_scoreLabels[i]) : null;
            ScoreLabels[i].Text = null;
        }
    }

    /// <summary>
    /// Updates all the labels text
    /// </summary>
    public virtual void OnScoresUpdated()
    {
        //Logger.LogDebug("scores updated");
        if (pinGod?.Players?.Count > 0)
        {            
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
                    else { scoreLabel.Text = pinGod.Player.Points.ToScoreString();}
                }
                else
                {
                    scoreLabel.Text = null;
                }
            }

            //in all player labels update their scores
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

            //update current player and ball
            if (ballInfolabel != null)
            {                
                ballInfolabel.Text = Tr("BALL") + " " + pinGod.BallInPlay.ToString();
            }
            if (playerInfoLabel != null)
            {
                playerInfoLabel.Text = $"{Tr("PLAYER")}: {pinGod.CurrentPlayerIndex + 1}";
                ballInfolabel.Text = Tr("BALL") + " " + (pinGod.BallInPlay).ToString();
            }

        }
        else
        {
            if (_isDebugMode)
            {
                if (Players.Length > 1 && !_show_main_score_multiplayer)
                {
                    scoreLabel.Text = null;
                }
                else
                {
                    scoreLabel.Text = (Players[0].Points).ToScoreString();
                }

                for (int i = 0; i < ScoreLabels.Length-1; i++)
                {
                    ScoreLabels[i].Text = (Players[i].Points).ToScoreString();
                }

                if (ballInfolabel != null)
                    ballInfolabel.Text = Tr("BALL") + " 3";

                if (playerInfoLabel != null)
                    playerInfoLabel.Text = Tr("PLAYER") + " PLAYER: 1";
            }
        }
    }

    /// <summary>
    /// override this but this will invoke <see cref="OnScoresUpdated"/> to update scene labels
    /// </summary>
    public virtual void OnBallStarted() => OnScoresUpdated();
}
