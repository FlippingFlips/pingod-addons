using Godot;
using PinGod.Base;
using PinGod.Core;
using PinGod.Core.Game;

/// <summary>
/// A bonus layer / mode. Display at end of ball. Bonus.tscn scene <para/>
/// Sends <see cref="PinGodBase.BonusEnded"/> signal on <see cref="_display_for_seconds"/> timeout
/// </summary>
public partial class Bonus : Control
{
    /// <summary>
    /// Text to use of none set in translation files under BONUS_EOB
    /// </summary>
    [Export] protected string _defaultText = string.Empty;

    /// <summary>
    /// Display for, default 5 secs
    /// </summary>
    [Export] protected float _display_for_seconds = 5;

    internal Label label;
    /// <summary>
    /// <see cref="PinGodGame"/> singleton
    /// </summary>
    protected IPinGodGame pinGod;
    /// <summary>
    /// Timer for bonus expire
    /// </summary>
    protected Timer timer;

    /// <summary>
    /// Sets up scene
    /// </summary>
    public override void _EnterTree()
    {
        if (HasNode(Paths.ROOT_PINGODGAME))
        {
            pinGod = GetNode<IPinGodGame>(Paths.ROOT_PINGODGAME);
        }
        Logger.Info(nameof(Bonus), ":_EnterTree");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Logger.Debug(nameof(Bonus), ":", nameof(_ExitTree));
    }

    /// <summary>
    /// Stops the <see cref="timer"/> is isn't stopped already
    /// </summary>
    public override void _Ready()
    {
        Logger.Info(nameof(Bonus), ":_Ready");
        //get nodes from this scene tree
        timer = GetNode("Timer") as Timer;
        label = GetNode("Label") as Label;

        if (string.IsNullOrWhiteSpace(_defaultText))
            _defaultText = Tr("BONUS_EOB");

        if (!timer.IsStopped())
            timer.Stop();
    }

    /// <summary>
    /// Stops the timer and emits the <see cref="PinGodBase.BonusEnded/>
    /// </summary>
    public virtual void OnTimedOut()
    {
        Logger.Info(nameof(Bonus), ":BonusEnded");
        timer.Stop();
        this.Visible = false;
        pinGod?.EmitSignal(nameof(PinGodBase.BonusEnded));
        this.QueueFree();
    }

    /// <summary>
    /// Creates bonus text to display with players bonus
    /// </summary>
    /// <returns></returns>
    public virtual string SetBonusText(string text = "")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            text = _defaultText;
        }

        //use extension method to create formatted score eg: "1,000,000"
        text += "\n" + pinGod?.Player?.Bonus.ToScoreString();

        return text;
    }

    /// <summary>
    /// Starts display for the amount of seconds set
    /// </summary>
    /// <param name="visible">can hide the display but still use timer to let game know bonus ended</param>
    public virtual void StartBonusDisplay(bool visible = true)
    {
        if (visible)
        {
            label.Text = SetBonusText();
            Logger.Debug(nameof(Bonus), ":set label text to:", label.Text);
        }

        Logger.Info(nameof(Bonus), ":displaying for " + _display_for_seconds);
        timer.Start(_display_for_seconds);
        Visible = visible;

        if (pinGod?.Player != null)
        {
            pinGod?.AddPoints(pinGod.Player.Bonus);
        }
    }
    /// <summary>
    /// Bonus has times out. Hide the display and send <see cref="PinGodBase.BonusEnded"/>
    /// </summary>
    private void _on_Timer_timeout()
    {
        OnTimedOut();
    }
}
