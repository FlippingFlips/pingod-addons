using Godot;
using System.Drawing;
using System.Threading.Tasks;
using static Godot.GD;

/// <summary>
/// Simple simulation of a Pinball ball trough. Handles trough switches, ball saves <para/>
/// Loaded as singleton on launch. Visual pinball uses the `bsTrough` in example.
/// Emits <see cref="PinGodGame"/> signals for ball saves, ball end
/// </summary>
public class Trough : Node
{
	/// <summary>
	/// Switch names, defaults trough_1 -- trough_4
	/// </summary>
	[Export] public string[] _trough_switches = { "trough_1", "trough_2", "trough_3", "trough_4" };
	/// <summary>
	/// Early switch ball save names. outlane_l outlane_r
	/// </summary>
	[Export] public string[] _early_save_switches = { "outlane_l", "outlane_r" };
	/// <summary>
	/// The solenoid / coil name to use when kicking the ball from the trough
	/// </summary>
	[Export] public string _trough_solenoid = "trough";
    /// <summary>
    /// The solenoid / coil name to use when kicking the ball in the plunger lane
    /// </summary>
    [Export] public string _auto_plunge_solenoid = "auto_plunger";
	/// <summary>
	/// THe switch name used for the plunger lane
	/// </summary>
	[Export] public string _plunger_lane_switch = "plunger_lane";
	/// <summary>
	/// The lamp name to cycle for ball saves
	/// </summary>
	[Export] public string _ball_save_lamp = "";
    /// <summary>
    /// The led name to cycle for ball saves
    /// </summary>
    [Export] public string _ball_save_led = "shoot_again";
	/// <summary>
	/// default ball save time
	/// </summary>
	[Export] public byte _ball_save_seconds = 8;
	/// <summary>
	/// default ball save in multi-ball
	/// </summary>
	[Export] public byte _ball_save_multiball_seconds = 8;
	/// <summary>
	/// number of balls to save, defaults to one single ball play
	/// </summary>
	[Export] public byte _number_of_balls_to_save = 1;
	/// <summary>
	/// Sets the <see cref="PinGodGame.BallStarted"/>
	/// </summary>
	[Export] public bool _set_ball_started_on_plunger_lane = true;
	/// <summary>
	/// Enables ball save when leaving plunger lane if ball is started, <see cref="PinGodGame.BallStarted"/>
	/// </summary>

	[Export] public bool _set_ball_save_on_plunger_lane = true;
	/// <summary>
	/// Use this to turn off trough checking, outside VP
	/// </summary>
	[Export] public bool _isDebugTrough = false;

	/// <summary>
	/// Ball saver timer. Setup in <see cref="_Ready"/>
	/// </summary>
	private Timer ballSaverTimer;
	private PinGodGame pinGod;
	private Timer troughPulseTimer;
	/// <summary>
	/// 
	/// </summary>
	public TroughOptions TroughOptions { get; set; }
	/// <summary>
	/// Total balls locked
	/// </summary>
    public int BallsLocked { get; internal set; }

    private int _mballSaveSecondsRemaining;
	/// <summary>
	/// Sets up timers in the scene for pulsing and ball saves. Sets up <see cref="TroughOptions"/>
	/// </summary>
	public override void _EnterTree()
	{
		pinGod = GetNode("/root/PinGodGame") as PinGodGame;
		ballSaverTimer = GetNode("BallSaverTimer") as Timer;
		troughPulseTimer = GetNode("TroughPulseTimer") as Timer;
		pinGod.LogInfo("trough:_enter tree");

		//trough options
		TroughOptions = new TroughOptions(_trough_switches, _trough_solenoid, _plunger_lane_switch,
			_auto_plunge_solenoid, _early_save_switches, _ball_save_seconds, _ball_save_multiball_seconds, _ball_save_lamp, _ball_save_led, _number_of_balls_to_save);
	}
	/// <summary>
	/// Listen for actions. Mainly from trough switches and plunger lanes.
	/// </summary>
	/// <param name="event"></param>
	public override void _Input(InputEvent @event)
	{
		//fire early saves
		if (pinGod.IsBallStarted && !pinGod.IsTilted && pinGod.BallSaveActive)
		{
			for (int i = 0; i < TroughOptions?.EarlySaveSwitches?.Length; i++)
			{
				if (pinGod.SwitchOn(TroughOptions.EarlySaveSwitches[i], @event))
				{
					FireEarlySave();
					pinGod.EmitSignal(nameof(PinGodGame.BallSaved));
					break;
				}
			}
		}

		//last switch (entry switch)
		if (IsTroughSwitchOn(@event))
		{
			var troughFull = IsTroughFull();
			if (troughFull && pinGod.IsBallStarted && !pinGod.IsMultiballRunning)
			{
				if (pinGod.BallSaveActive && !pinGod.IsTilted)
				{
					pinGod.LogDebug("trough: ball_saved");
					pinGod.SolenoidPulse(TroughOptions.Coil);
					pinGod.EmitSignal(nameof(PinGodGame.BallSaved));
				}
				else
				{
					pinGod.LogDebug("trough: ball_drained");
					pinGod.EmitSignal(nameof(PinGodGame.BallDrained));
				}
			}
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
		if (IsTroughSwitchOff(@event)) { } //to record the switch off

		if (!pinGod.GameInPlay) return;
		if (pinGod.IsTilted) return;

		//when the ball is in the plunger lane stop any ball search running TODO
		if (pinGod.SwitchOn(TroughOptions.PlungerLaneSw, @event))
		{
			//auto plunge the ball if in ball save or game is tilted to get the balls back
			if (pinGod.BallSaveActive || pinGod.IsTilted || pinGod.IsMultiballRunning)
			{
				pinGod.SolenoidPulseTimer(TroughOptions.AutoPlungerCoil, 500);
				pinGod.LogDebug("trough: auto saved");
			}
		}
		//reset the ball search when leaving the switch
		else if (pinGod.SwitchOff(TroughOptions.PlungerLaneSw, @event))
		{			
			//start a ball saver if game in play
			if (pinGod.GameInPlay && !pinGod.BallStarted && !pinGod.IsTilted && !pinGod.IsMultiballRunning)
			{
				if(_set_ball_started_on_plunger_lane)
					pinGod.BallStarted = true;

				if(_set_ball_save_on_plunger_lane)
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
	/// <summary>
	/// Just debug logs the amount of switches set in the <see cref="TroughOptions"/>
	/// </summary>
	public override void _Ready()
	{
		pinGod.LogDebug("trough:_ready. switch_count: ", TroughOptions?.Switches.Length);
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
			if (pinGod.SwitchOn(TroughOptions.Switches[i]))
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
    /// Checks every switch in <see cref="TroughOptions.Switches"/> for enabled. If <see cref="_isDebugTrough"/> then will return full
    /// </summary>
    public bool IsTroughFull()
	{
		if (_isDebugTrough)
			return true;

		var isFull = true;
		for (int i = 0; i < TroughOptions.Switches.Length-BallsLocked; i++)
		{			
			if (!pinGod.SwitchOn(TroughOptions.Switches[i]))
			{
				isFull = false;
				break;
			}

            Logger.LogDebug("trough: isTroughFull: " + TroughOptions.Switches[i] + " active");
        }
		return isFull;
	}

	/// <summary>
	/// Pulse the ball trough
	/// </summary>
	public void PulseTrough() => pinGod.SolenoidPulse(TroughOptions.Coil);

    /// <summary>
    /// Activates the ball saver if not already running. Blinks the ball saver lamp
    /// </summary>
    /// <returns>True if the ball saver is active</returns>
    public bool StartBallSaver(float seconds = 8)
	{
		pinGod.LogDebug($"trough: ball_save_started " + seconds);
		ballSaverTimer.Stop();
		pinGod.BallSaveActive = true;
		ballSaverTimer.Start(seconds);
		UpdateLamps(LightState.Blink);		
		return pinGod.BallSaveActive;
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

	void _startMballTrough(float delay) => troughPulseTimer.Start(delay);

	private void FireEarlySave()
	{
		pinGod.LogDebug("trough:early ball_saved");
		PulseTrough();
		pinGod.EmitSignal(nameof(PinGodGame.BallSaved));
	}

	bool IsTroughSwitchOn(InputEvent input)
	{
		for (int i = 0; i < TroughOptions.Switches.Length; i++)
		{
			if (pinGod.SwitchOn(TroughOptions.Switches[i], input))
				return true;
		}

		return false;
	}

	bool IsTroughSwitchOff(InputEvent input)
	{
		for (int i = 0; i < TroughOptions.Switches.Length; i++)
		{
			if (pinGod.SwitchOff(TroughOptions.Switches[i], input))
				return true;
		}

		return false;
	}

	private void OnMultiballStarted()
	{				
		pinGod.LogDebug("trough: mball starting save for ", _mballSaveSecondsRemaining);
		CallDeferred("_startMballTrough", 1f);
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
	/// Disables ball save and emits <see cref="PinGodGame.BallSaveEnded"/>
	/// </summary>
	void _on_BallSaverTimer_timeout()
	{
		DisableBallSave();
		pinGod.EmitSignal(nameof(PinGodGame.BallSaveEnded));
	}
	void _trough_pulse_timeout()
	{
		_mballSaveSecondsRemaining--;
		if (_mballSaveSecondsRemaining < 1)
		{
			DisableBallSave();			
			pinGod.LogDebug("trough: ended mball trough pulse timer");
		}

		//put a ball into plunger_lane
		if (!pinGod.SwitchOn(TroughOptions.PlungerLaneSw) && TroughOptions.BallSaveSeconds > 0)
		{
			var ballsIntTrough = BallsInTrough();
			var b = TroughOptions.Switches.Length - ballsIntTrough;
			if (b < TroughOptions.NumBallsToSave)
			{
				pinGod.LogDebug("trough: pulse");
				PulseTrough();
			}
		}		
	}
	#endregion
}
