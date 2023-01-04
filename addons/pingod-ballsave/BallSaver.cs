using Godot;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

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
    [Signal] public delegate void BallSavedEventHandler(bool earlySwitch = false);
    #endregion

    public float TimeRemaining;
    private bool _ballSaveActive;
    private PinGodMachine _machine;
    private bool _inGracePeriod = false;

    public override void _EnterTree()
    {
        base._EnterTree();
        //set wait time 1 to repeat so we can count down the remaining time
        OneShot = false; WaitTime = 1;
        //hook to the timeout
        Timeout += BallSaver_Timeout;

        //looking for machine
        if (HasNode("/root/Machine"))
        {
            _machine = GetNode<PinGodMachine>("/root/Machine");
            Logger.Debug(nameof(BallSave), ":Found Machine Node");
        }
    }

    public override void _Ready()
    {
        base._Ready();
        if(_machine != null) _machine.SwitchCommand += _machine_SwitchCommand;
    }

    public override void _ExitTree()
    {
        if (_machine != null) _machine.SwitchCommand -= _machine_SwitchCommand;
    }

    private void _machine_SwitchCommand(string name, byte index, byte value)
    {
        if (_ballSaveActive && _early_save_switches.Contains(name))
        {
            //Logger.WarningRich("[code=yellow]", nameof(Trough), ": plugin requires PinGodGame to act on BallSaved: ", swName, "[/color]");
            //FireEarlySave();
            //emit ball saved early switch
            EmitSignal(nameof(BallSaved), true);
        }
    }

    private void BallSaver_Timeout()
    {
        TimeRemaining -= 1.0f;
        //Logger.Verbose(nameof(BallSaver), $": wait time: {WaitTime}, remaining: {TimeRemaining}");
        if (TimeRemaining <= 0f)
        {
            if (!_inGracePeriod)
            {
                _inGracePeriod = true;
                UpdateLamps(LightState.Off);
                TimeRemaining += _ball_save_grace_seconds;
                Logger.Debug(nameof(BallSaver), ": added grace period of " + _ball_save_grace_seconds, ", new remaining time: " + TimeRemaining);
                return;
            }

            if (_inGracePeriod)
            {
                Logger.Debug(nameof(BallSaver), ": BallSaverTimer ended, disabling saves");
                DisableBallSave();
            }            
        }
    }

    /// <summary>
    /// Disable ball saves and turns off lamps
    /// </summary>
    public void DisableBallSave()
    {
        this.Stop();
        Logger.Debug(nameof(BallSaver), nameof(DisableBallSave));
        _ballSaveActive = false;
        //troughPulseTimer.Stop();        
        UpdateLamps(LightState.Off);
        EmitSignal(nameof(BallSaveDisabled));
    }

    public virtual bool IsBallSaveActive() => _ballSaveActive;

    /// <summary>
    /// Activates the ball saver if not already running. Blinks the ball saver lamp
    /// </summary>
    /// <returns>True if the ball saver is active</returns>
    public bool StartSaver(float seconds = 0)
    {
        seconds = seconds > 0 ? seconds : _ball_save_seconds;
        TimeRemaining = seconds;
        _ballSaveActive = true;
        Logger.Debug(nameof(BallSaver), $":Start. Remaining:" + seconds," secs. Starting timer and lights.");
        this.Stop();
        this.Start(1); //this object is a timer
        UpdateLamps(LightState.Blink);
        EmitSignal(nameof(BallSaveEnabled));
        return true;
    }

    public bool StartSaverMultiball(float seconds = 0)
    {
        seconds = seconds > 0 ? seconds : _ball_save_multiball_seconds;
        Logger.Debug(nameof(BallSaver), $": starting multi-ball saves");
        return StartSaver(seconds);
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
