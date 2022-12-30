using Godot;
using System.Drawing;
using static PinGodBase;

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
		Logger.Debug(nameof(Trough), ":_EnterTree");

		//trough options
		TroughOptions = new TroughOptions(_trough_switches, _trough_solenoid, _plunger_lane_switch,
			_auto_plunge_solenoid, _early_save_switches, _ball_save_seconds, _ball_save_multiball_seconds, _ball_save_lamp, _ball_save_led, _number_of_balls_to_save);

		//switch commands
		pinGod.Connect(nameof(PinGodBase.SwitchCommand), this, nameof(OnSwitchCommand));
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
			//check the early
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
	}

	private void OnPlungerSwitchHandler(byte value)
	{
		if (!pinGod.GameInPlay) return;
		if (pinGod.IsTilted) return;

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

	/// <summary>
	/// Trough switch command handler
	/// </summary>
	/// <param name="swName"></param>
	/// <param name="index"></param>
	/// <param name="value"></param>
	private void OnTroughSwitchCommand(string swName, byte index, byte value)
	{
		Logger.Verbose(nameof(Trough), $":{nameof(OnTroughSwitchCommand)}:{index}-{value}");

		//trough switch enabled
		if (value > 0)
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
			Logger.Verbose(nameof(Trough), ":off : ", swName);
		}
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
	public void PulseTrough() => pinGod.SolenoidPulse(TroughOptions.Coil);

	/// <summary>
	/// Activates the ball saver if not already running. Blinks the ball saver lamp
	/// </summary>
	/// <returns>True if the ball saver is active</returns>
	public bool StartBallSaver(float seconds = 8)
	{
		Logger.Debug(nameof(Trough), $":ball_save_started:" + seconds);
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

	void _startMballTrough(float delay)
	{
		Logger.Debug(nameof(Trough), ":start mball trough pulse timer: " + delay);
		troughPulseTimer.Start(delay);
	}

	private void FireEarlySave()
	{
		Logger.Debug(nameof(Trough), $": {nameof(FireEarlySave)}");
		PulseTrough();
		pinGod.EmitSignal(nameof(BallSaved));
	}

	bool IsTroughActionSwitchOn(InputEvent input)
	{
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

		//put a ball into plunger_lane
		if (!Machine.Switches[TroughOptions.PlungerLaneSw].IsEnabled && TroughOptions.BallSaveSeconds > 0)
		//if (!pinGod.SwitchOn(TroughOptions.PlungerLaneSw) && TroughOptions.BallSaveSeconds > 0)
		{            
            var ballsIntTrough = BallsInTrough();
			var b = TroughOptions.Switches.Length - ballsIntTrough;
            Logger.Debug(nameof(Trough), ":balls in trough="+ballsIntTrough+$":ball={b}:numToSave:{TroughOptions.NumBallsToSave}");
            if (b < TroughOptions.NumBallsToSave)
			{				
				PulseTrough();
			}
		}
	}
	#endregion
}
