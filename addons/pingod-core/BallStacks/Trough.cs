using Godot;
using PinGod.Base;
using PinGod.Core.Service;

namespace PinGod.Core.BallStacks
{
    /// <summary>
    /// Simple simulation of a Pinball ball trough. Handles trough switches, ball saves <para/>
    /// Loaded as singleton on launch. Visual pinball uses the `bsTrough` in example.
    /// Emits <see cref="PinGodGame"/> signals for ball saves, ball end
    /// </summary>
    public partial class Trough : Node
    {
        private MachineNode _machine;

        /// <summary>
        /// Enables ball save when leaving plunger lane if ball is started, <see cref="PinGodGame.BallStarted"/>
        /// </summary>
        /// <summary>
        /// Ball saver timer. Setup in <see cref="_Ready"/>
        /// </summary>
        private Timer ballSaverTimer;
        private IPinGodGame pinGod;
        private Timer troughPulseTimer;
        /// <summary>
        /// Total balls locked
        /// </summary>
        public int BallsLocked { get; set; }

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

            if (!_isEnabled)
            {
                Logger.Info(nameof(Trough), ":_EnterTree", " trough is disabled in scene, freeing up");
                this.QueueFree();
                return;
            }

            troughPulseTimer = new Timer { Name = "TroughPulseTimer", OneShot = false, WaitTime = 1 };
            AddChild(troughPulseTimer);
            troughPulseTimer.Timeout += _trough_pulse_timeout;

            Logger.Debug(nameof(Trough), ":added TroughPulseTimer");

            //trough options
            TroughOptions = new TroughOptions(_trough_switches, _trough_solenoid);

            Logger.Debug(nameof(Trough), ":_EnterTree: setup complete");
        }

        public override void _ExitTree()
        {
            base._ExitTree();
        }

        /// <summary>
        /// Just debug logs the amount of switches set in the <see cref="TroughOptions"/>
        /// </summary>
        public override void _Ready()
        {
            if (!_isEnabled)
            {
                Logger.Info(nameof(Trough), ":_Ready", " trough is disabled in scene, freeing up");
                this.QueueFree();
                return;
            }

            Logger.Debug(nameof(Trough), ":_ready. switch_count: ", TroughOptions?.Switches.Length);

            if (HasNode("/root/PinGodGame"))
            {
                pinGod = GetNode("/root/PinGodGame") as IPinGodGame;
                Logger.Debug(nameof(Trough), ":Found " + nameof(IPinGodGame));
            }

            //looking for machine config
            if (GetParent().HasNode("/root/Machine"))
            {
                _machine = GetParent().GetNode<MachineNode>("/root/Machine");
                Logger.Debug(nameof(Trough), ":Found Machine Node");
            }

            if (_machine != null)
            {
                Logger.Debug(nameof(Trough), ":" + nameof(_Ready), ": hooking on to /root/Machine switch commands");
                _machine.SwitchCommand += OnSwitchCommand;//switch commands
            }
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
                if (TroughOptions.GameSwitches[i].IsEnabled())
                {
                    cnt++;
                }
            }
            return cnt;
        }

        /// <summary>
        /// Checks every switch in <see cref="TroughOptions.GameSwitches"/> for IsEnabled. <para/>
        /// If <see cref="_isDebugTrough"/> then will return IsFull true
        /// </summary>
        public bool IsTroughFull()
        {
            if (_isDebugTrough)
            {
                Logger.Verbose(nameof(Trough), ": DEBUG TROUGH IS ON, always full, switch off for simulator.");
                return true;
            }                

            if (TroughOptions?.GameSwitches?.Count <= 0)
            {
                Logger.Warning(nameof(Trough), ":no GameSwitches set");
                return false;
            }

            var isFull = true;

            for (int i = 0; i < TroughOptions.GameSwitches.Count - BallsLocked; i++)
            {
                if (!TroughOptions.GameSwitches[i].IsEnabled())
                {
                    isFull = false;
                    break;
                }
            }

            //Logger.Verbose(nameof(Trough), ":isTroughFull: " + isFull);
            return isFull;
        }

        /// <summary>
        /// Pulse the ball trough
        /// </summary>
        public void PulseTrough() => _machine.CoilPulse(TroughOptions.Coil);

        /// <summary>
        /// Starts multi-ball trough
        /// </summary>
        /// <param name="numOfBalls">num of balls to save</param>
        /// <param name="ballSaveTime"></param>
        /// <param name="pulseTimerDelay">Timer to pulse trough</param>
        public void StartMultiball(byte numOfBalls, byte ballSaveTime = 0, float pulseTimerDelay = 1)
        {
            if (_machine?._ballSaver != null)
            {
                var balls = numOfBalls == 0 ? 1 : numOfBalls;
                _machine._ballSaver._number_of_balls_to_save = (byte)balls;

                //if leave save time to zero it will use the default set value
                _machine?._ballSaver.StartSaverMultiball(ballSaveTime);
            }

            if (pulseTimerDelay > 0)
                _startMballTrough(pulseTimerDelay);

            OnMultiballStarted();
        }

        void _startMballTrough(float delay)
        {
            Logger.Debug(nameof(Trough), ":start m-ball trough pulse timer delay: " + delay);
            troughPulseTimer.Start(delay);
        }

        private void FireEarlySave()
        {
            Logger.Debug(nameof(Trough), $": {nameof(FireEarlySave)}");
            PulseTrough();
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

        private void OnMultiballStarted() => CallDeferred(nameof(_startMballTrough), 1f);

        void OnSwitchCommand(string swName, byte index, byte value)
        {
            var on = value > 0;

            if (string.IsNullOrEmpty(swName) && index > 0)
            {
                swName = Machine.Switches?.GetSwitch(index)?.Name;
            }

            if (swName.Contains("trough")) //trough switch handler
            {
                Logger.Verbose($"{nameof(Trough)}:{nameof(OnSwitchCommand)}", " trough on switch: " + swName);
                OnTroughSwitchCommand(swName, index, value);
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
            //Logger.Verbose(nameof(Trough), $":{nameof(OnTroughSwitchCommand)}:{index}-{value}");
            //trough switch enabled and PinGodGame is available
            if (value > 0)
            {
                if (pinGod != null)
                {
                    var troughFull = IsTroughFull();
                    //trough full
                    if (troughFull && pinGod.BallStarted && !pinGod.IsMultiballRunning)
                    {
                        if (_machine?.IsBallSaveActive() ?? false && !pinGod.IsTilted)
                        {
                            Logger.Debug(nameof(Trough), ":ball_saved");
                            PulseTrough();
                            pinGod.EmitSignal("BallSaved");
                        }
                        else
                        {
                            Logger.Debug(nameof(Trough), ":ball_drained");
                            pinGod.EmitSignal("BallDrained");
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
                            pinGod.EmitSignal("MultiBallEnded");
                        }
                    }
                }
                else
                {
                    Logger.WarningRich("[color=yellow]", nameof(Trough), ":", swName, ":" + "plugin requires IPinGodGame to act on sending BallSaved, BallDrained, MultiBallEnded: switch ", swName, "[/color]");
                }
            }
            else
            {
                Logger.Verbose(nameof(Trough), ":off : ", swName);
            }
        }

        #region Timers

        /// <summary>
        /// Timer for feeding balls into plunger lane when in a multi-ball from the trough
        /// </summary>
        void _trough_pulse_timeout()
        {
            //Logger.Debug(nameof(Trough), ": trough pulse time out");
            if (_machine?._plungerLane != null && _machine._ballSaver != null)
            {
                var sw = Machine.Switches[_machine._plungerLane._plunger_lane_switch];
                //Logger.Verbose(nameof(Trough), ": plunger lane time since:", sw.TimeSinceChange(), " is on=", sw.IsEnabled(), " ball saver time=", _machine._ballSaver.TimeRemaining);

                //ball isn't in plunger lane and ball saver on then put ball into lane
                if (!_machine._plungerLane.IsSwitchActive())
                {
                    if(_machine._ballSaver.TimeRemaining > 0)
                    {
                        var ballsIntTrough = BallsInTrough();
                        var b = TroughOptions.Switches.Length - ballsIntTrough;
                        Logger.Debug(nameof(Trough), ":balls in trough=" + ballsIntTrough + $":ball={b}:numToSave:{TroughOptions.NumBallsToSave}");
                        if (b < _machine._ballSaver._number_of_balls_to_save)
                        {
                            PulseTrough();
                        }
                    }                    
                }
                else { Logger.Debug(nameof(Trough), ": plunger lane is active, can't put ball in lane while active."); }
            }
            else { Logger.Debug(nameof(Trough), ": No plunger lane or ball saver nodes found"); }
        }
        #endregion
    }

}