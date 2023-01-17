using Godot;
using PinGod.Base;
using PinGod.Core.Interface;
using PinGod.Core.Service;
using System.Collections.Generic;

namespace PinGod.Core
{
    public interface IPinGodGame
    {
        /// <summary>
        /// Adjustments for game. Set by autoload plug-in Adjustments
        /// </summary>
        Adjustments Adjustments { get; }
        /// <summary>
        /// Audio resources and functions
        /// </summary>
        IAudioManager AudioManager { get; }
        /// <summary>
        /// Current number of the ball in play
        /// </summary>
        byte BallInPlay { get; set; }
        /// <summary>
        /// Is ball save active
        /// </summary>
        bool BallSaveActive { get; }
        /// <summary>
        /// Set options for game ball search
        /// </summary>
        BallSearchOptions BallSearchOptions { get; set; }
        /// <summary>
        byte BallsPerGame { get; set; }
        /// <summary>
        /// Flag used for ball saving on the initial launch. If ball enters the shooterlane after a ball save then this is to stop the ballsave starting again
        /// </summary>
        bool BallStarted { get; set; }
        Error Connect(StringName signal, Callable callable, uint flags = 0);
        /// <summary>
        /// Coin credits
        /// </summary>
        public int Credits { get; set; }
        /// <summary>
        /// Index of current player, 0 for player_1
        /// </summary>
        byte CurrentPlayerIndex { get; set; }
        /// <summary>
        /// Flippers enabled?
        /// </summary>
        bool FlippersEnabled { get; }

        Audits Audits { get; }
        /// <summary>
        /// Game is in play?
        /// </summary>
        bool GameInPlay { get; set; }
        /// <summary>
        /// Total game time
        /// </summary>
        ulong GetElapsedGameTime { get; }
        /// <summary>
        /// Gets the highest score made from the <see cref="Audits.HighScores"/>
        /// </summary>
        long GetTopScorePoints { get; }
        /// <summary>
        /// Is the game in bonus mode
        /// </summary>
        bool InBonusMode { get; set; }
        /// <summary>
        /// Any multi-ball running?
        /// </summary>
        bool IsMultiballRunning { get; set; }
        bool IsPlayerEnteringHighscore { get; set; }
        /// <summary>
        /// Is Game tilted?
        /// </summary>
        bool IsTilted { get; set; }
        /// <summary>
        /// Logging levels. Default to info
        /// </summary>
        LogLevel LogLevel { get; set; }
        /// <summary>
        /// Machine node reference. Should be autoloaded as Machine
        /// </summary>
        MachineNode MachineNode { get; }
        /// <summary>
        /// Current player. Base <see cref="PinGodPlayer"/>
        /// </summary>
        IPinGodPlayer Player { get; }
        /// <summary>
        /// <see cref="IPinGodPlayer"/> Players List for current Game
        /// </summary>
        List<IPinGodPlayer> Players { get; set; }
        /// <summary>
        /// <see cref="IPinGodPlayer"/> Players List from the last game
        /// </summary>
        List<IPinGodPlayer> PlayersLastGame { get; set; }
        /// <summary>
        /// Game has been set to quit
        /// </summary>
        bool QuitRequested { get; }
        /// <summary>
        /// Amount of tilt warnings the machine is on.
        /// </summary>
        byte Tiltwarnings { get; set; }        

        void _EnterTree();

        void _ExitTree();

        void _Input(InputEvent @event);

        void _Ready();

        /// <summary>
        /// Adds bonus to the current player
        /// </summary>
        /// <param name="points"></param>
        void AddBonus(long points);
        /// <summary>
        /// Adds credits to the Audits and emits <see cref="PinGodBase.CreditAdded"/> signal
        /// </summary>
        /// <param name="amt"></param>
        void AddCredits(byte amt);
        /// <summary>
        /// Adds points to the current player
        /// </summary>
        /// <param name="points"></param>
        /// <param name="emitUpdateSignal">Sends a <see cref="PinGodBase.ScoresUpdated"/> signal if set. See <see cref="PinGodBase.ScoreMode"/> for use</param>
        long AddPoints(long points, bool emitUpdateSignal = true);
        /// <summary>
        /// Gets balls in play from the <see cref="_trough"/>
        /// </summary>
        /// <returns></returns>
        int BallsInPlay();
        /// <summary>
        /// Creates a new <see cref="PinGodPlayer"/>. Override this for your own custom players
        /// </summary>
        /// <param name="name"></param>
        void CreatePlayer(string name);
        /// <summary>
        /// Starts the <see cref="Trough.StartBallSaver(float)"/> ball saver
        /// </summary>
        /// <param name="secs"></param>
        //internal void BallSaveEnabled(float secs) => _trough?.StartSaver(secs);
        /// <summary>
        /// <see cref="Machine.DisableAllLamps"/>
        /// </summary>
        void DisableAllLamps();
        /// <summary>
        /// <see cref="Machine.DisableAllLeds"/>
        /// </summary>
        void DisableAllLeds();
        /// <summary>
        /// Emit Godot signal from this instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        Error EmitSignal(StringName signal, params Variant[] args);
        /// <summary>
        /// Sets the Enable flippers on/off. coil:"flippers" <see cref="Machine.SetCoil(string, bool)"/>
        /// </summary>
        /// <param name="enabled"></param>
        void EnableFlippers(bool enabled);
        /// <summary>
        /// Ends the current ball. Changes the player <para/>
        /// Emits <see cref="PinGodBase.BallEnded"/> signal
        /// </summary>
        /// <returns>True if all balls finished, game is finished</returns>
        bool EndBall();
        /// <summary>
        /// Game has ended, sets <see cref="GameInPlay"/> to false and Sends <see cref="GameEnded"/>
        /// </summary>
        void EndOfGame();
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        ulong GetLastSwitchChangedTime(string sw);
        /// <summary>
        /// Gets the Resources autoloaded scene at "/root/Resources"
        /// </summary>
        /// <returns></returns>
        //Resources GetResources();
        /// <summary>
        /// Detect if the input `isAction` found in the given switchNames. Uses <see cref="SwitchActionOn(string, InputEvent)"/>
        /// </summary>
        /// <param name="switchNames"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        bool IsSwitchAction(string[] switchNames, InputEvent input);
        /// <summary>
        /// Checks the <see cref="Machine.Switches"/> for enabled
        /// </summary>
        /// <param name="swName"></param>
        /// <returns></returns>
        bool IsSwitchEnabled(string swName);
        /// <summary>
        /// <see cref="Audits.Load"/>
        /// </summary>
        void LoadDataFile();
        /// <summary>
        /// Exported games load patches from res://patch/patch_{patchNum}.pck . From 1. patch_1.pck, patch_2.pck
        /// </summary>
        void LoadPatches();
        /// <summary>
        /// Wrapper <see cref="Logger"/>
        /// </summary>
        /// <param name="what"></param>
        void LogDebug(params object[] what);
        void LogError(string message = null, params object[] what);
        void LogInfo(params object[] what);
        void LogWarning(string message = null, params object[] what);
        /// <summary>
        /// Invokes OnBallDrained on all groups marked as Mode within the scene tree.
        /// </summary>
        /// <param name="sceneTree"></param>
        /// <param name="group"></param>
        /// <param name="method"></param>
        void OnBallDrained(SceneTree sceneTree, string group = "Mode", string method = "OnBallDrained");
        /// <summary>
        /// Invokes OnBallSaved on all groups marked as Mode within the scene tree.
        /// </summary>
        /// <param name="sceneTree"></param>
        /// <param name="group"></param>
        /// <param name="method"></param>
        void OnBallSaved(SceneTree sceneTree, string group = "Mode", string method = "OnBallSaved");
        /// <summary>
        /// Invokes OnBallStarted on all groups marked as Mode within the scene tree.
        /// </summary>
        /// <param name="sceneTree"></param>
        /// <param name="group"></param>
        /// <param name="method"></param>
        void OnBallStarted(SceneTree sceneTree, string group = "Mode", string method = "OnBallStarted");
        /// <summary>
        /// Uses the <see cref="AudioManager.PlayMusic(string, float)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        void PlayMusic(string name, float pos = 0);
        /// <summary>
        /// Uses the <see cref="AudioManager.PlaySfx(string, string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bus">defaults to bus named Sfx</param>
        void PlaySfx(string name, string bus = "Sfx");
        /// <summary>
        ///  <see cref="AudioManager.PlayVoice(string, string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bus">Plays on bus name</param>
        void PlayVoice(string name, string bus = "Voice");
        /// <summary>
        /// Quits the game, saves data / settings and cleans up
        /// </summary>
        /// <param name="saveData">save game on exit?</param>
        void Quit(bool saveData = true);
        /// <summary>
        /// Use random number generator from range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        int RandomNumber(int from, int to);
        /// <summary>
        /// Reset the game
        /// </summary>
        void ResetGame();
        /// <summary>
        /// Reset player warnings and tilt
        /// </summary>
        void ResetTilt();
        /// <summary>
        /// Sets lamp state and state inside the <see cref="_lampMatrixOverlay"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        void SetLampState(string name, byte state);
        /// <summary>
        /// Sets state of led
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="color"></param>
        void SetLedState(string name, byte state, System.Drawing.Color? colour = null);
        /// <summary>
        /// Sets led states from System.Drawing Color
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="colour"></param>
        void SetLedState(string name, byte state, Color? colour = null);
        /// <summary>
        /// Sets led state from godot color
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="colour"></param>
        void SetLedState(string name, byte state, int color = 0);
        /// <summary>
        /// Sets led from RGB
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        void SetLedState(string name, byte state, byte r, byte g, byte b);
        /// <summary>
        /// Set IsEnabled on the Switch and emits <see cref="PinGodBase.SwitchCommand"/> with number and byte value <para/>
        /// Switch will be set to 0 or 1 but the signal value can be 0-255 <para/>
        /// Anything listening to the <see cref="PinGodBase.SwitchCommand"/> can get the Switch from <see cref="Machine.Switches"/> without checking keys, it will be valid sent from here
        /// </summary>
        /// <param name="swNum"></param>
        /// <param name="value"></param>
        void SetSwitch(int swNum, byte value);
        /// <summary>
        /// Sets switch when not coming from a godot action. Sets switch and Emits <see cref="PinGodBase.SwitchCommand"/>.
        /// </summary>
        /// <param name="switch"></param>
        /// <param name="value"></param>
        /// <param name="fromAction">if false it doesn't set switch, actions should do this when checked here in SwitchOn</param>
        void SetSwitch(Switch @switch, byte value, bool fromAction = true);
        /// <summary>
        /// Sets up machine items from the collections, starts memory mapping and recordings
        /// </summary>
        void Setup();
        /// <summary>
        /// <see cref="Machine.SetCoil(name, state)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        void SolenoidOn(string name, byte state);
        /// <summary>
        /// Pulses a <see cref="Machine.Coils"/> by name. Creates a new task?
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pulse"></param>
        void SolenoidPulse(string name, byte pulse = 255);
        /// <summary>
        /// Creates a timer and removes it rather than new tasks. Need more testing if working 100%, todo: send multiple times see what it does.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pulse"></param>
        void SolenoidPulseTimer(string name, float pulse = 0.255F);

        /// <summary>
        /// Attempts to start a game. If games in play then add more players until <see cref="MaxPlayers"/> <para/>
        /// </summary>
        /// <returns>True if the game was started</returns>
        bool StartGame();
        /// <summary>
        /// Sets MultiBall running in the trough and Emits <see cref="PinGodBase.MultiballStarted"/>
        /// </summary>
        /// <param name="numOfBalls">Number of balls to save. A 2 ball multiball would be 2</param>
        /// <param name="ballSaveTime"></param>
        /// <param name="pulseTime">Delay for the trough in multi-ball</param>
        void StartMultiBall(byte numOfBalls, byte ballSaveTime = 20, float pulseTime = 0);
        /// <summary>
        /// Starts a new ball, changing to next player, enabling flippers and ejecting trough and sending <see cref="BallStarted"/>
        /// </summary>
        void StartNewBall();
        /// <summary>
        /// Stop music <see cref="AudioManager.StopMusic"/>
        /// </summary>
        /// <returns>position in seconds was stopped. -1 if no <see cref="AudioManager"/> was found</returns>
        float StopMusic();
        /// <summary>
        /// Checks a switches input event by friendly name that is in the <see cref="Switches"/> <para/>
        /// "coin", @event
        /// </summary>
        /// <param name="swName"></param>
        /// <param name="inputEvent"></param>
        /// <returns></returns>
        bool SwitchActionOff(string swName, InputEvent inputEvent);
        /// <summary>
        /// Use in Godot _Input events. Checks a switches input event by friendly name from switch collection <para/>
        /// "coin", @event
        /// </summary>
        /// <param name="swName"></param>
        /// <param name="inputEvent"></param>
        /// <returns></returns>
        bool SwitchActionOn(string swName, InputEvent inputEvent);
        /// <summary>
        /// Checks a switches input event by friendly name. <para/>
        /// If the "coin" switch is still held down then will return true
        /// </summary>
        /// <param name="swName"></param>
        /// <returns>on / off</returns>
        bool SwitchOn(string swName);
        /// <summary>
        /// Invokes UpdateLamps on all groups marked as Mode within the scene tree. scene tree CallGroup
        /// </summary>
        /// <param name="sceneTree"></param>
        /// <param name="group"></param>
        /// <param name="method"></param>
        void UpdateLamps(SceneTree sceneTree, string group = "Mode", string method = "UpdateLamps");        
    }
}