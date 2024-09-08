using Godot;
using PinGod.Base;
using System.Drawing;
using System.Linq;

namespace PinGod.Core
{
    public partial class BallSaver : Timer
    {
        #region Exports
        [Export] public bool _isPluginEnabled = true;
        /// <summary>
        /// default Ball save time
        /// </summary>
        [Export] public byte _ball_save_seconds = 8;
        /// <summary>
        /// Ball save grace time
        /// </summary>
        [Export] public byte _ball_save_grace_seconds = 2;
        /// <summary>
        /// Allow multiple
        /// </summary>
        [Export] public bool _allowMutlipleSaves = true;
        /// <summary>
        /// The lamp name to cycle for Ball saves
        /// </summary>
        [Export] public string _ball_save_lamp = "";
        /// <summary>
        /// The led name to cycle for Ball saves
        /// </summary>
        [Export] public string _ball_save_led = "shoot_again";
        /// <summary>
        /// default Ball save in multi-Ball
        /// </summary>
        [Export] public byte _ball_save_multiball_seconds = 8;
        /// <summary>
        /// number of balls to save, defaults to one single Ball play
        /// </summary>
        [Export] public byte _number_of_balls_to_save = 1;
        /// <summary>
        /// Early switch Ball save names. outlane_l outlane_r
        /// </summary>
        [Export] public string[] _early_save_switches = { "outlaneL", "outlaneR" };
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
        private MachineNode _machine;
        private bool _inGracePeriod = false;

        public override void _EnterTree()
        {
            base._EnterTree();

            if (!_isPluginEnabled)
            {
                Logger.Info(nameof(BallSaver), ":plugin is disabled in scene, removing");
                this.QueueFree();
                return;
            }

            //set wait time 1 to repeat so we can count down the remaining time
            OneShot = false; WaitTime = 1;
            //hook to the timeout
            Timeout += BallSaver_Timeout;

            //looking for machine
            if (HasNode(Paths.ROOT_MACHINE))
            {
                _machine = GetNode<MachineNode>(Paths.ROOT_MACHINE);
                Logger.Debug(nameof(BallSaver), ":Found Machine Node");
            }
        }

        public override void _Ready()
        {
            base._Ready();
            if (_machine != null) _machine.SwitchCommand += _machine_SwitchCommand;
        }

        public override void _ExitTree()
        {
            if (_machine != null) _machine.SwitchCommand -= _machine_SwitchCommand;
        }

        /// <summary>
        /// Handles the early switches
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void _machine_SwitchCommand(string name, byte index, byte value)
        {
            if (_ballSaveActive)
            {
                if (_early_save_switches.Contains(name))
                {
                    //emit Ball saved early switch
                    EmitSignal(nameof(BallSaved), true);

                    if (!_allowMutlipleSaves) DisableBallSave();
                }               
            }
        }

        /// <summary>
        /// Decrements ball save TimeRemaining by 1 second <para/>
        /// When times out adds _ball_save_grace_seconds for extension
        /// </summary>
        protected virtual void BallSaver_Timeout()
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
        /// Disable Ball saves and turns off lamps
        /// </summary>
        public void DisableBallSave()
        {
            this.Stop();
            Logger.Debug(nameof(BallSaver), nameof(DisableBallSave));
            _ballSaveActive = false;

            //reset number to save
            _number_of_balls_to_save = 1;

            //troughPulseTimer.Stop();        
            UpdateLamps(LightState.Off);
            EmitSignal(nameof(BallSaveDisabled));
        }

        private void FireEarlySave() => Logger.Debug(nameof(BallSaver), $": {nameof(FireEarlySave)}");

        public virtual bool IsBallSaveActive() => _ballSaveActive;

        /// <summary>
        /// Activates the Ball saver if not already running. Blinks the Ball saver lamp
        /// </summary>
        /// <returns>True if the Ball saver is active</returns>
        public bool StartSaver(float seconds = 0, bool? allowMultipleSaves = null)
        {            
            _allowMutlipleSaves = allowMultipleSaves.HasValue ? allowMultipleSaves.Value : _allowMutlipleSaves;
            seconds = seconds > 0 ? seconds : _ball_save_seconds;
            TimeRemaining = seconds;
            _ballSaveActive = true;
            Logger.Debug(nameof(BallSaver), $":Start. Remaining:" + seconds, " secs. Starting timer and lights.");
            this.Stop();
            this.Start(1); //this object is a timer
            UpdateLamps(LightState.Blink);
            EmitSignal(nameof(BallSaveEnabled));
            return true;
        }

        public bool StartSaverMultiball(float seconds = 0, bool allowMultipleSaves = true)
        {
            _allowMutlipleSaves = allowMultipleSaves;
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
                Machine.SetLamp(_ball_save_lamp, (byte)state);
            else if (!string.IsNullOrWhiteSpace(_ball_save_led))
                Machine.SetLed(_ball_save_led, (byte)state, ColorTranslator.ToOle(System.Drawing.Color.Green));
        }

        public override string ToString() =>
            $"MultiSave:{_allowMutlipleSaves}\nTime:{TimeRemaining}\nInGrace:{_inGracePeriod}";
    }
}
