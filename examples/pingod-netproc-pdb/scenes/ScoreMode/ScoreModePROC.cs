using PinGod.Base;
using PinGod.Modes;

/// <summary>
/// Custom Godot mode to update score display
/// </summary>
public partial class ScoreModePROC : ScoreMode
{
    private PinGodGameProc _pinGodProc;
    PinGodProcGameController _procGame;

    public override void _Ready()
    {
        base._Ready();
        _pinGodProc = pinGod as PinGodGameProc;
        if (_pinGodProc != null) _procGame = _pinGodProc.PinGodProcGame;
    }

    /// <summary>
    /// don't use base method, override, the base method updates from PinGod players.
    /// </summary>
    public override void UpdateMainScore()
    {
        //base.UpdateMainScore(); 

        if (this.scoreLabel != null)
        {
            var cp = _procGame.Players[_procGame.CurrentPlayerIndex];
            if(cp.Score > -1)
            {
                //more than 1 player, show main score?
                if (_procGame.Players.Count > 1 && !_show_main_score_multiplayer)
                {
                    scoreLabel.Text = null;
                }
                //set main score label
                else { scoreLabel.Text = cp.Score.ToScoreString(); }
            }
            else { scoreLabel.Text = null; }
        }
    }

    public override void UpdatePlayerBallInfo()
    {
        if (_procGame?.Players?.Count <= 0) return;

        if (ballInfolabel != null)
        {
            ballInfolabel.Text = Tr("BALL") + " " + _procGame.Ball.ToString();
        }
        if (playerInfoLabel != null)
        {
            playerInfoLabel.Text = $"{Tr("PLAYER")}: {_procGame.CurrentPlayerIndex + 1}";
        }
    }

    public override void UpdatePlayerScores()
    {
        //base.UpdatePlayerScores();
        int i = 0;
        foreach (var player in _procGame?.Players)
        {
            //this hides displaying a multi-player score for the single player.
            if (_pinGodProc.Players.Count == 1 && i == 0 && !_single_player_p1_visible)
            {
                i++;
                continue;
            }
            var lbl = ScoreLabels[i];
            if (lbl != null)
            {
                if (player.Score > -1)
                {
                    lbl.Text = player.Score.ToScoreString();
                }
                else
                {
                    lbl.Text = null;
                }
            }
            i++;
        }
    }

    public override void OnScoresUpdated()
    {
        if(_procGame?.Players?.Count > 0)
        {
            //want to call this base as it invokes the other update methods we can override
            base.OnScoresUpdated();
        }
    }
}
