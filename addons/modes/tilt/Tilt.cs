using Godot;
using PinGod.Core;
using PinGod.Core.BallStacks;
using PinGod.Core.Game;
using PinGod.Core.Service;
using PinGod.EditorPlugins;

/// <summary>
/// Tilt. "slam_tilt" or "tilt" switches.
/// </summary>
public partial class Tilt : Control
{

    protected IPinGodGame pinGod;

    /// <summary>
    /// Time machine waits to end after tilted
    /// </summary>
    [Export] byte _inTiltedSeconds = 5;

    private MachineNode _machine;

    /// <summary>
    /// How many warnings a player is allowed before tilting
    /// </summary>
    [Export] byte _num_tilt_warnings = 2;

    bool _slamTilted;

    BlinkingLabel blinkingLayer;

    float displayForSecs = 2f;

    //protected Timer timer;

    private Trough trough;
    private Tween _tween;

    /// <summary>
    /// Emitted signal when game is tilted
    /// </summary>
    [Signal] public delegate void GameTiltedEventHandler();

    public override void _ExitTree()
    {
        Logger.Debug(nameof(Tilt), ":", nameof(_ExitTree));
        if(_machine!=null) _machine.SwitchCommand -= OnSwitchCommand;
        blinkingLayer?.QueueFree();
        if(_tween!=null) _tween.Finished -= _on_Timer_timeout;
        base._ExitTree();        
    }

    /// <summary>
    /// Sets Visible to false
    /// </summary>
    public virtual void _on_Timer_timeout()
    {
        Visible = false;
        if (_slamTilted)
        {
            //reset the game
            //var ms = GetNodeOrNull<MainScene>("/root/MainScene");
            //ms?.ResetGame();
            Input.ParseInputEvent(new InputEventAction { Action = "reset", Pressed = true });
        }
    }
    /// <summary>
    /// Gets access to the game and the trough. Gets the timer and label to show if tilted
    /// </summary>
    public override void _Ready()
	{
        Logger.Debug(nameof(Tilt), ":", nameof(_Ready));
		//hide this mode
		Visible = false;

        if (HasNode("/root/PinGodGame"))
        {
            pinGod = GetNode("/root/PinGodGame") as IPinGodGame;
            _num_tilt_warnings = pinGod.Adjustments.TiltWarnings;
        }

        if (HasNode("/root/Trough"))
        {
            trough = GetNode<Trough>("/root/Trough");
        }

        //switch commands
        if (HasNode("/root/Machine"))
        {
            _machine = GetNode<MachineNode>("/root/Machine");
            _machine.SwitchCommand += OnSwitchCommand;
        }
        else { Logger.WarningRich(nameof(Tilt), ": [color=yellow]", "no Machine node in root found", "[/color]"); }

        //text layer to display warnings and tilted
        blinkingLayer = GetNode("CenterContainer/BlinkingLabel") as BlinkingLabel;
		blinkingLayer.Text = "";
    }

    /// <summary>
    /// Hides the text, stops timer if running
    /// </summary>
    public virtual void OnBallStarted()
    {
        if (_tween?.IsRunning() ?? false) { _tween.Stop(); }
        SetText("");
        Visible = false;
    }

    /// <summary>
    /// Slam tilt game. Disable flippers. TODO: Should reset game
    /// </summary>
    public virtual void OnSlamTilt()
    {
        _tween?.Stop();
        Logger.Info(nameof(Tilt), ":slam tilt");
        SetText(Tr("SLAMTILT"));
        pinGod.PlaySfx("tilt");
        pinGod.IsTilted = true;
        pinGod.EnableFlippers(false);
        Visible = true;
        CreateTiltTween(_inTiltedSeconds);
        _slamTilted = true;
        _machine?.DisableBallSaver();        
    }

    /// <summary>
    /// Switch was activated so act on them here
    /// </summary>
    /// <param name="swName"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public virtual void OnSwitchCommand(string swName, byte index, byte value)
    {
        Logger.Debug("switch " + swName);
        if (!pinGod.GameInPlay) return;
        if (pinGod.IsTilted) return;
        var on = value > 0;
        if (on)
        {
            switch (swName)
            {
                case "tilt":
                    OnTilt();
                    break;
                case "slam_tilt":
                    OnSlamTilt();
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Warnings and tilts if warnings go over limit of the <see cref="PinGodGame.Tiltwarnings"/>
    /// </summary>
    public virtual void OnTilt()
    {        
        Logger.Info(nameof(Tilt), ":tilt active");
        if (_tween?.IsRunning() ?? false) { _tween?.Stop(); }

        //add a warning
        pinGod.Tiltwarnings++;
        //set tilted
        if (pinGod.Tiltwarnings > _num_tilt_warnings)
        {
            pinGod.IsTilted = true;
            pinGod.EnableFlippers(false);
            Visible = true;
            Logger.Info(nameof(Tilt), ":game tilted");
            ShowTilt();
            _machine?.DisableBallSaver();
            EmitSignal(nameof(GameTilted));
        }
        //show warnings
        else
        {
            ShowWarning();
        }
    }
    /// <summary>
    /// Sets text of the BlinkingLayer
    /// </summary>
    /// <param name="text"></param>
    public virtual void SetText(string text) => blinkingLayer.Text = text;

    /// <summary>
    /// Sets a blinking layer text label using the translate message TILT to languages available
    /// </summary>
    public virtual void ShowTilt()
    {        
        pinGod.PlaySfx("tilt");
        _tween.Stop();
        //stop the timer for showing tilt information
        CallDeferred(nameof(SetText), Tr("TILT"));
    }

    /// <summary>
    /// Show the player how many tilt warnings
    /// </summary>
    public virtual void ShowWarning()
    {
        CreateTiltTween(displayForSecs);
        pinGod?.PlaySfx("warning");
        CallDeferred(nameof(SetText), $"{Tr("TILT_WARN")} {pinGod.Tiltwarnings}");
        Visible = true;
    }

    void CreateTiltTween(float secs)
    {
        if (_tween != null)
        {
            _tween.Stop();
            _tween.Finished -= _on_Timer_timeout;
        }        
        _tween = CreateTween();
        _tween.Finished += _on_Timer_timeout;
        _tween.SetLoops(1);
        _tween.TweenInterval(secs);

    }
}
