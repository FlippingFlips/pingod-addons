using Godot;
using System.Drawing;
using System.Net.NetworkInformation;

public partial class BallSaver : Timer
{
    #region Exports
    /// <summary>
    /// default ball save time
    /// </summary>
    [Export] public byte _ball_save_seconds = 8;

    /// <summary>
    /// ball save grace time
    /// </summary>
    [Export] public byte _ball_save_grace_seconds = 2;

    /// <summary>
    /// The lamp name to cycle for ball saves
    /// </summary>
    [Export] public string _ball_save_lamp = "";

    /// <summary>
    /// The led name to cycle for ball saves
    /// </summary>
    [Export] public string _ball_save_led = "shoot_again";

    /// <summary>
    /// default ball save in multi-ball
    /// </summary>
    [Export] public byte _ball_save_multiball_seconds = 8;

    /// <summary>
    /// number of balls to save, defaults to one single ball play
    /// </summary>
    [Export] public byte _number_of_balls_to_save = 1;

    /// <summary>
    /// Early switch ball save names. outlane_l outlane_r
    /// </summary>
    [Export] public string[] _early_save_switches = { "outlane_l", "outlane_r" };
    #endregion

    #region Signals
    /// <summary>
    /// Ball save disabled, timed out
    [Signal] public delegate void BallSaveDisabledEventHandler();
    [Signal] public delegate void BallSaveEnabledEventHandler(); 
    #endregion

    private bool _ballSaveActive;

    public override void _EnterTree()
    {
        base._EnterTree();
        OneShot = true;
        WaitTime = _ball_save_seconds;
        Timeout += BallSaver_Timeout;
    }

    private void BallSaver_Timeout()
    {
        Logger.Debug(nameof(BallSaver), ": BallSaverTimer ended, disabling saves");
        DisableBallSave();
    }

    /// <summary>
    /// Disable ball saves and turns off lamps
    /// </summary>
    public void DisableBallSave()
    {
        Logger.Debug(nameof(Trough), nameof(DisableBallSave));        
        _ballSaveActive = false;
        //troughPulseTimer.Stop();
        UpdateLamps(LightState.Off);
        EmitSignal(nameof(BallSaveDisabled));
    }

    /// <summary>
    /// Activates the ball saver if not already running. Blinks the ball saver lamp
    /// </summary>
    /// <returns>True if the ball saver is active</returns>
    public bool StartBallSaver(float seconds = 0)
    {
        seconds = seconds > 0 ? seconds : _ball_save_seconds;
        _ballSaveActive = true;
        Logger.Debug(nameof(Trough), $":_ballSaveActive=" + seconds," starting timer, lights");
        this.Start(seconds); //this object is a timer
        UpdateLamps(LightState.Blink);
        EmitSignal(nameof(BallSaveEnabled));
        return true;
    }

    /// <summary>
    /// Sets the shoot again lamp / or led state
    /// </summary>
    /// <param name="state"></param>
    private void UpdateLamps(LightState state)
    {
        if (!string.IsNullOrWhiteSpace(_ball_save_lamp))
        {
            Machine.SetLamp(_ball_save_lamp, (byte)state);
        }
        else if (!string.IsNullOrWhiteSpace(_ball_save_led))
        {
            Machine.SetLed(_ball_save_led, (byte)state, ColorTranslator.ToOle(System.Drawing.Color.Yellow));
        }
    }
}
