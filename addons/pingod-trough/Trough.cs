using Godot;
using System.Drawing;
using static PinGodBase;

/// <summary>
/// Simple simulation of a Pinball ball trough. Handles trough switches, ball saves <para/>
/// Loaded as singleton on launch. Visual pinball uses the `bsTrough` in example.
/// Emits <see cref="PinGodGame"/> signals for ball saves, ball end
/// </summary>
public partial class Trough : Node
{
    private PinGodMachine _machineConfig;

    private int _mballSaveSecondsRemaining;

    /// <summary>
    /// Enables ball save when leaving plunger lane if ball is started, <see cref="PinGodGame.BallStarted"/>
    /// </summary>
    /// <summary>
    /// Ball saver timer. Setup in <see cref="_Ready"/>
    /// </summary>
    private Timer ballSaverTimer;
	private PinGodGame pinGod;
	private Timer troughPulseTimer;
    /// <summary>
    /// Total balls locked
    /// </summary>
    public int BallsLocked { get; internal set; }

    /// <summary>
    /// 
    /// </summary>
    public TroughOptions TroughOptions { get; set; }
    /// <summary>
    /// Sets up timers in the scene for pulsing and ball saves. Sets up <see cref="TroughOptions"/>
    /// </summary>
    public override void _EnterTree()
	{
        Logger.Debug(nameof(Trough), ":_EnterTree");

        ballSaverTimer = new Timer { Name = "BallSaverTimer", OneShot=true, WaitTime=1 };
        AddChild(ballSaverTimer);
        troughPulseTimer = new Timer { Name = "TroughPulseTimer", OneShot =false, WaitTime = 1 };
        AddChild(troughPulseTimer);
        Logger.Debug(nameof(Trough), ":added BallSaverTimer|TroughPulseTimer");

        if (HasNode("/root/PinGodGame"))
		{
            pinGod = GetNode("/root/PinGodGame") as PinGodGame;
            Logger.Debug(nameof(Trough), ":Found " + nameof(PinGodGame));
        }

        //looking for machine config
		if (GetParent().HasNode("Machine"))
		{
			_machineConfig = GetParent().GetNode<PinGodMachine>("Machine");
			_machineConfig.SwitchCommand += OnSwitchCommand;//switch commands
            Logger.Debug(nameof(Trough), ":Found Machine Node");
        }

        //trough options
        TroughOptions = new TroughOptions(_trough_switches, _trough_solenoid, _plunger_lane_switch,
			_auto_plunge_solenoid, _early_save_switches, _ball_save_seconds, _ball_save_multiball_seconds, _ball_save_lamp, _ball_save_led, _number_of_balls_to_save);

        Logger.Debug(nameof(Trough), ":_EnterTree: setup complete");
    }

    /// <summary>
    /// Just debug logs the amount of switches set in the <see cref="TroughOptions"/>
    /// </summary>
    public override void _Ready()
    {
        Logger.Debug(nameof(Trough), ":_ready. switch_count: ", TroughOptions?.Switches.Length);
    }

    /// <summary>
    /// Returns the BallsInPlay by checking the amount currently in the trough
    /// </summary>
    /// <returns></returns>
    public int BallsInPlay() => TroughOptions.Switches.Length - BallsInTrough();

    /// <summary>
    /// Counts the number of trough switches currently active
    /// </summary>
    /// <returns></returns>
    public int BallsInTrough()
    {
        var cnt = 0;
        for (int i = 0; i < TroughOptions.Switches.Length; i++)
        {
            if (Machine.Switches[TroughOptions.Switches[i]].IsEnabled)
            {
                cnt++;
            }
        }
        return cnt;
    }

    /// <summary>
    /// Disable ball saves and turns off lamps
    /// </summary>
    public void DisableBallSave()
    {
        pinGod.BallSaveActive = false;
        troughPulseTimer.Stop();
        UpdateLamps(LightState.Off);
    }

    /// <summary>
    /// Checks every switch in <see cref="TroughOptions.GameSwitches"/> for IsEnabled. <para/>
    /// If <see cref="_isDebugTrough"/> then will return IsFull true
    /// </summary>
    public bool IsTroughFull()
    {
        if (_isDebugTrough)
            return true;

        if(TroughOptions?.GameSwitches?.Count <=0)
        {
            Logger.Warning(nameof(Trough), ":no GameSwitches set");
            return false;
        }

        var isFull = true;

        for (int i = 0; i < TroughOptions.GameSwitches.Count - BallsLocked; i++)
        {
            if (!TroughOptions.GameSwitches[i].IsEnabled)
            {
                isFull = false;
                break;
            }
        }

        Logger.Verbose(nameof(Trough), ":isTroughFull: " + isFull);
        return isFull;
    }

    /// <summary>
    /// Pulse the ball trough
    /// </summary>
    public void PulseTrough() => _machineConfig.CoilPulseTimer(TroughOptions.Coil);

    /// <summary>
    /// Activates the ball saver if not already running. Blinks the ball saver lamp
    /// </summary>
    /// <returns>True if the ball saver is active</returns>
    public bool StartBallSaver(float seconds = 8)
    {
        Logger.Debug(nameof(Trough), $":ball_save_started:" + seconds);
        ballSaverTimer.Stop();
        ballSaverTimer.Start(seconds);
        UpdateLamps(LightState.Blink);
        if (pinGod != null)
        {
            pinGod.BallSaveActive = true;            
        }

        return true;
    }

    /// <summary>
    /// Starts multi-ball trough
    /// </summary>
    /// <param name="numOfBalls">num of balls to save</param>
    /// <param name="ballSaveTime"></param>
    /// <param name="pulseTimerDelay">Timer to pulse trough</param>
    public void StartMultiball(byte numOfBalls, byte ballSaveTime, float pulseTimerDelay = 1)
    {
        TroughOptions.MballSaveSeconds = ballSaveTime;
        TroughOptions.NumBallsToSave = numOfBalls;

        _mballSaveSecondsRemaining = TroughOptions.MballSaveSeconds;
        StartBallSaver(TroughOptions.MballSaveSeconds);

        if (pulseTimerDelay > 0)
            _startMballTrough(pulseTimerDelay);

        OnMultiballStarted();
    }

    void _startMballTrough(float delay)
    {
        Logger.Debug(nameof(Trough), ":start mball trough pulse timer: " + delay);
        troughPulseTimer.Start(delay);
    }

    private void FireEarlySave()
    {
        Logger.Debug(nameof(Trough), $": {nameof(FireEarlySave)}");
        PulseTrough();
        pinGod?.EmitSignal(nameof(PinGodBase.BallSaved));
    }

    bool IsTroughActionSwitchOn(InputEvent input)
    {
        //TODO: dont think needed anymore
        for (int i = 0; i < TroughOptions.Switches.Length; i++)
        {
            if (pinGod.SwitchActionOn(TroughOptions.Switches[i], input))
            {
                var sw = Machine.Switches[TroughOptions.Switches[i]];
                pinGod.SetSwitch(sw, 1, true);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Godot actions for trough off. Set switch will be called
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    bool IsTroughSwitchOff(InputEvent input)
    {
        //TODO: dont think needed anymore
        for (int i = 0; i < TroughOptions.Switches.Length; i++)
        {
            if (pinGod.SwitchActionOff(TroughOptions.Switches[i], input))
            {
                var sw = Machine.Switches[TroughOptions.Switches[i]];
                pinGod.SetSwitch(sw, 0);
                return true;
            }
        }

        return false;
    }

    private void OnMultiballStarted()
    {
        Logger.Debug(nameof(Trough), ": mball starting save for ", _mballSaveSecondsRemaining);
        CallDeferred("_startMballTrough", 1f);
    }

    private void OnPlungerSwitchHandler(byte value)
    {
        if (!pinGod?.GameInPlay ?? false) return;
        if (pinGod?.IsTilted ?? true) return;

        if(pinGod != null)
        {
            //switch on
            if (value > 0)
            {
                //auto plunge the ball if in ball save or game is tilted to get the balls back
                if (pinGod.BallSaveActive || pinGod.IsMultiballRunning)
                {
                    pinGod.SolenoidPulse(TroughOptions.AutoPlungerCoil);
                    Logger.Verbose(nameof(Trough), ":auto plunger saved");
                }
            }
            //switch off
            else
            {
                //start a ball saver if game in play
                if (pinGod.GameInPlay && !pinGod.BallStarted && !pinGod.IsTilted && !pinGod.IsMultiballRunning)
                {
                    if (_set_ball_started_on_plunger_lane)
                        pinGod.BallStarted = true;

                    if (_set_ball_save_on_plunger_lane)
                    {
                        var saveStarted = StartBallSaver(TroughOptions.BallSaveSeconds);
                        if (saveStarted)
                        {
                            UpdateLamps(LightState.Blink);
                            pinGod.EmitSignal(nameof(PinGodGame.BallSaveStarted));
                        }
                    }
                }
            }
        }        
    }

    void OnSwitchCommand(string swName, byte index, byte value)
    {
		var on = value > 0;
		if (swName.Contains("trough")) //trough switch handler
		{
			Logger.Verbose($"{nameof(Trough)}:{nameof(OnSwitchCommand)}", "trough on switch: " + swName);
			OnTroughSwitchCommand(swName, index, value);
		}
		else if (swName == _plunger_lane_switch) //ball save auto plunger, plunger_lane
		{
			OnPlungerSwitchHandler(value);
		}
		else
		{
            //check the early save switches
            if (pinGod != null)
			{                
                if (pinGod.IsBallStarted && !pinGod.IsTilted && pinGod.BallSaveActive)
                {
                    for (int i = 0; i < TroughOptions?.EarlySaveSwitches?.Length; i++)
                    {
                        if (TroughOptions.EarlySaveSwitches[i] == swName)
                        {
                            FireEarlySave();
                            pinGod.EmitSignal(nameof(PinGodGame.BallSaved));
                            break;
                        }
                    }
                }
            }
            else
            {
                Logger.WarningRich("[code=yellow]", nameof(Trough), ": plugin requires PinGodGame to act on BallSaved: ", swName, "[/color]");
            }
		}
	}
	/// <summary>
	/// Trough switch command handler
	/// </summary>
	/// <param name="swName"></param>
	/// <param name="index"></param>
	/// <param name="value"></param>
	private void OnTroughSwitchCommand(string swName, byte index, byte value)
	{
		Logger.Verbose(nameof(Trough), $":{nameof(OnTroughSwitchCommand)}:{index}-{value}");

		//trough switch enabled and PinGodGame is available
		if (value > 0)
		{
            if(pinGod != null)
            {
                var troughFull = IsTroughFull();
                //trough full
                if (troughFull && pinGod.IsBallStarted && !pinGod.IsMultiballRunning)
                {
                    if (pinGod.BallSaveActive && !pinGod.IsTilted)
                    {
                        Logger.Debug(nameof(Trough), ":ball_saved");
                        pinGod.SolenoidPulse(TroughOptions.Coil);
                        pinGod.EmitSignal(nameof(PinGodGame.BallSaved));
                    }
                    else
                    {
                        Logger.Debug(nameof(Trough), ":ball_drained");
                        pinGod.EmitSignal(nameof(PinGodGame.BallDrained));
                    }
                }
                //multiball
                else if (pinGod.IsMultiballRunning && !pinGod.BallSaveActive)
                {
                    var balls = BallsInTrough();
                    if (TroughOptions.Switches.Length - 1 == balls)
                    {
                        pinGod.IsMultiballRunning = false;
                        troughPulseTimer.Stop();
                        pinGod.EmitSignal(nameof(PinGodGame.MultiBallEnded));
                    }
                }
            }
            else
            {
                Logger.WarningRich("[code=yellow]",nameof(Trough), ": plugin requires PinGodGame to act on ballsaved, drained, ballsaves, multiball : ", swName,"[/color]");
            }
		}
		else
		{
			Logger.Verbose(nameof(Trough), ":off : ", swName);
		}
	}
	/// <summary>
	/// Sets the shoot again lamp / or led state
	/// </summary>
	/// <param name="state"></param>
	private void UpdateLamps(LightState state)
	{
		if (!string.IsNullOrWhiteSpace(TroughOptions.BallSaveLamp))
		{
			pinGod.SetLampState(TroughOptions.BallSaveLamp, (byte)state);
		}
		else if (!string.IsNullOrWhiteSpace(TroughOptions.BallSaveLed))
		{
			pinGod.SetLedState(TroughOptions.BallSaveLed, (byte)state, ColorTranslator.ToOle(System.Drawing.Color.Yellow));
		}
	}

	#region Timers
	/// <summary>
	/// Disables ball save and emits <see cref="PinGodBase.BallSaveEnded"/>
	/// </summary>
	void _on_BallSaverTimer_timeout()
	{
		DisableBallSave();
		pinGod.EmitSignal(nameof(PinGodGame.BallSaveEnded));
	}

	/// <summary>
	/// Timer for pulsing balls from trough
	/// </summary>
	void _trough_pulse_timeout()
	{
		_mballSaveSecondsRemaining--;
		if (_mballSaveSecondsRemaining < 1)
		{
			DisableBallSave();			
			Logger.Debug(nameof(Trough), ": ended mball trough pulse timer");
		}

        //ball is in plunger lane
        if (Machine.IsSwitchOn(TroughOptions.PlungerLaneSw) && TroughOptions.BallSaveSeconds > 0)
        {
            var ballsIntTrough = BallsInTrough();
            var b = TroughOptions.Switches.Length - ballsIntTrough;
            Logger.Debug(nameof(Trough), ":balls in trough=" + ballsIntTrough + $":ball={b}:numToSave:{TroughOptions.NumBallsToSave}");
            if (b < TroughOptions.NumBallsToSave)
            {
                PulseTrough();
            }
        }
	}
	#endregion
}
