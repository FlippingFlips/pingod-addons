using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Create class inheriting this and use the PinGodGame.tscn. <para/>
/// This should be set to AutoLoad with the corresponding scene for games, see Project Settings <para/>
/// Has <see cref="Trough"/>
/// </summary>
public abstract partial class PinGodGame : PinGodBase, IPinGodGame
{
    #region Exports
    [Export] bool _lamp_overlay_enabled = false;
    [Export] bool _playback_game = false;
    [Export] string _playbackfile = null;
    [Export] bool _record_game = false;
    [Export] bool _switch_overlay_enabled = false;
    #endregion

    #region Fields
    /// <summary>
    /// Maximum players on machine
    /// </summary>
    public byte MaxPlayers = 4;
    internal Trough _trough;
    /// <summary>
    /// Update lamps overlay. <see cref="LampMatrix"/>
    /// </summary>
    protected LampMatrix _lampMatrixOverlay = null;
    /// <summary>
    /// The memory map used to communicate states between software
    /// </summary>
    protected PinGodMemoryMapNode memMapping;
    private Queue<PlaybackEvent> _playbackQueue;
    /// <summary>
    /// recording actions to file using godot
    /// </summary>
    private FileAccess _recordFile;
    private RecordPlaybackOption _recordPlayback;
    private ulong gameEndTime;
    private ulong gameLoadTimeMsec;
    private ulong gameStartTime;
    private MainScene mainScene;
    private RandomNumberGenerator randomNumGenerator = new RandomNumberGenerator(); 
    #endregion

    /// <summary>
    /// Initializes and creates empty player list
    /// </summary>
    public PinGodGame()
    {
        Players = new List<PinGodPlayer>();
    }

    #region Properties
    public Adjustments Adjustments { get; internal set; }
    public AudioManager AudioManager { get; protected set; }
    public byte BallInPlay { get; set; }
    public bool BallSaveActive { get; internal set; }
    public BallSearchOptions BallSearchOptions { get; set; }
    public byte BallsPerGame { get; set; }
    public bool BallStarted { get; internal set; }
    /// <summary>
    /// Commands args incoming from <see cref="GetCommandLineArgs"/>
    /// </summary>
    public Dictionary<string, string> CmdArgs { get; private set; }
    public byte CurrentPlayerIndex { get; set; }
    public bool FlippersEnabled { get; private set; }
    public GameData GameData { get; internal set; }
    public bool GameInPlay { get; set; }
    public virtual ulong GetElapsedGameTime => gameEndTime - gameStartTime;
    public virtual long GetTopScorePoints => GameData?.HighScores?
            .OrderByDescending(x => x.Scores).FirstOrDefault().Scores ?? 0;
    public bool InBonusMode { get; set; } = false;
    public bool IsBallStarted { get; internal set; }
    public bool IsMultiballRunning { get; set; } = false;
    public bool IsTilted { get; set; }
    public LogLevel LogLevel { get; set; } = LogLevel.Info;
    public PinGodMachine PinGodMachine { get; private set; }
    public PinGodPlayer Player { get; private set; }
    public List<PinGodPlayer> Players { get; set; }
    public bool QuitRequested { get; private set; }
    public byte Tiltwarnings { get; set; }
    #endregion

    #region Godot overrides    
    /// <summary>
    /// Gets user cmd line args, loads data and settings, creates trough, sets up ball search and audio manager
    /// </summary>
    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
            //LogDebug(nameof(PinGodGame), $":_EnterTree. {PinGodGameAddOn.VERSION}");
            CmdArgs = GetCommandLineArgs();

            //adjustments from root
            LogDebug(nameof(PinGodGame), nameof(_EnterTree), ": looking for adjustments module...");
            if (HasNode("/root/Adjustments"))
            {
                Adjustments = GetNode<AdjustmentsScript>("/root/Adjustments")?._adjustments ?? null;
                if (Adjustments != null)
                    LogInfo(nameof(PinGodGame), nameof(_EnterTree), ": adjustments loaded.");
            }
            else { LogDebug(nameof(PinGodGame), nameof(_EnterTree), ": adjustments module not found..."); }


            Logger.LogLevel = Adjustments?.LogLevel ?? LogLevel.Warning;
            LogInfo(nameof(PinGodGame), ":_EnterTree. log level: " + Logger.LogLevel);

            SetupWindow();
            LoadPatches();
            LoadDataFile();

            //get trough from tree
            if (HasNode("/root/Trough"))
            {
                _trough = GetNodeOrNull<Trough>("/root/Trough");
            }

            Setup();
            SetupAudio();

            LogInfo(nameof(PinGodGame), ":enter tree setup complete");
            gameLoadTimeMsec = Godot.Time.GetTicksMsec();

            //get the machine plugin. Setup ball search.
            var machineNode = $"/root/Machine";
            LogDebug($"{nameof(PinGodGame)}: _Ready|looking for {machineNode}");
            if (this.HasNode(machineNode))
            {
                LogDebug($"{nameof(PinGodGame)}: _Ready| MachineConfig found");
                PinGodMachine = GetNode<PinGodMachine>(machineNode);
                BallSearchOptions = PinGodMachine.BallSearchOptions;
                PinGodMachine.SwitchCommand += PinGodMachine_SwitchCommand;

                if (_trough == null && PinGodMachine.HasNode("Trough"))
                {
                    _trough = PinGodMachine.GetNode("Trough") as Trough;
                }
            }
            else { Logger.WarningRich("[color=yellow]", $"{nameof(PinGodGame)}: _Ready| PinGodMachine not found", "[/color]"); }

            if (_trough == null) Logger.WarningRich("[color=yellow]", nameof(PinGodGame), ": trough not found", "[/color]");
        }
    }
    /// <summary>
    /// Saves recordings if enabled and runs <see cref="Quit(bool)"/>
    /// </summary>
    public override void _ExitTree()
    {
        if (_recordPlayback == RecordPlaybackOption.Record)
        {
            LogInfo(nameof(PinGodGame), ":_ExitTree, saving recordings");
            SaveRecording();
        }
        else LogInfo(nameof(PinGodGame), ":_ExitTree");

        Quit(true);
    }
    /// <summary>
    /// Listens for any escape action preses. Handles coin switches, adds credits. Toggle border F2 default
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        //quits the game. ESC
        if (InputMap.HasAction("quit"))
        {
            if (@event.IsActionPressed("quit"))
            {
                LogDebug(nameof(PinGodGame), ":quit action request. quitting whole tree.");
                LogInfo(nameof(PinGodGame), ":sent game ended coil: alive 0");

                //todo: commented out, is it needed? :)
                //SetGameResumed();
                GetTree().Quit(0);
                return;
            }
        }

        //if (InputMap.HasAction("pause"))
        //{
        //    if (@event.IsActionPressed("pause"))
        //    {
        //        //if (!settingsDisplay?.Visible ?? false)
        //        TogglePause();
        //        return;
        //    }
        //}
    }
    /// <summary>
    /// Processes playback events...Processing is disabled if it isn't enabled and playback is finished
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
        if (_recordPlayback != RecordPlaybackOption.Playback)
        {
            SetProcess(false);
            LogInfo(nameof(PinGodGame), ":_Process loop stopped. No recordings are being played back.");
            return;
        }
        else
        {
            if (_playbackQueue?.Count <= 0)
            {
                LogInfo(nameof(PinGodGame), ":playback events ended");
                _recordPlayback = RecordPlaybackOption.Off;
                return;
            }

            var time = Time.GetTicksMsec() - gameLoadTimeMsec;
            var nextEvt = _playbackQueue.Peek().Time;
            if (nextEvt <= time)
            {
                var pEvent = _playbackQueue.Dequeue();
                var ev = new InputEventAction() { Action = pEvent.EvtName, Pressed = pEvent.State };
                Input.ParseInputEvent(ev);
                LogInfo(nameof(PinGodGame), ":playback evt ", pEvent.EvtName);
            }
        }
    }
    /// <summary>
    /// Game initialized. Memory map is created here if read and write is enabled. <para/>  Gets <see cref="BallSearchOptions"/>, sets up a <see cref="_lampMatrixOverlay"/> <para/>
    /// Gets hold of the <see cref="MainScene"/> to control window size, stretch
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        //setup main scene if there is one
        var mainScenePath = $"/root/{nameof(MainScene)}";
        LogDebug($"{nameof(PinGodGame)}: _Ready|looking for node at: " + mainScenePath);
        if (this.HasNode(mainScenePath))
            mainScene = GetNodeOrNull<MainScene>(mainScenePath);
        else
            LogDebug($"{nameof(PinGodGame)}: _Ready|node not found at " + mainScenePath);

        //setup lamp overlay if there is one
        if (_lampMatrixOverlay != null)
        {
            LogDebug($"{nameof(PinGodGame)}: _Ready|setting labels for Lamp matrix overlay lamps");
            foreach (var item in Machine.Lamps)
            {
                _lampMatrixOverlay.SetLabel(item.Value.Num, item.Key);
            }
        }

        LogInfo(nameof(PinGodGame), ":sent pingod game ready coil: alive 1");
        Machine.SetCoil("alive", 1);
    }
    #endregion

    #region Public_Methods
    public virtual void AddBonus(long points)
    {
        if (Player != null)
        {
            Player.Bonus += points;
        }
    }
    public virtual void AddCredits(byte amt)
    {
        GameData.Credits += amt;
        EmitSignal(nameof(CreditAdded), GameData.Credits);
    }
    public virtual long AddPoints(long points, bool emitUpdateSignal = true)
    {
        if (Player != null)
        {
            Player.Points += points;
            if (emitUpdateSignal)
                EmitSignal(nameof(ScoresUpdated));
        }

        return points;
    }
    public virtual int BallsInPlay() => _trough?.BallsInPlay() ?? 0;
    public virtual void CreatePlayer(string name) => Players.Add(new PinGodPlayer() { Name = name, Points = 0 });
    public virtual void DisableAllLamps() => Machine.DisableAllLamps();
    public virtual void DisableAllLeds() => Machine.DisableAllLeds();
    public virtual void EnableFlippers(bool enabled)
    {
        FlippersEnabled = enabled;
        Machine.SetCoil("flippers", enabled);
    }
    public virtual bool EndBall()
    {
        if (!GameInPlay) return false;

        LogDebug(nameof(PinGodGame), ":EndBall. disabling flippers");
        IsBallStarted = false;
        BallStarted = false;
        EnableFlippers(false);

        if (Players.Count > 0)
        {
            LogInfo(nameof(PinGodGame), ":end of ball. current ball:" + BallInPlay);
            if (Player.ExtraBalls > 0)
            {
                LogDebug(nameof(PinGodGame), ": player has extra balls");
                this.EmitSignal(nameof(BallEnded), false);
            }
            else
            {
                if (Players.Count > 1)
                {
                    CurrentPlayerIndex++;
                    if (CurrentPlayerIndex + 1 > Players.Count)
                    {
                        CurrentPlayerIndex = 0;
                        BallInPlay++;
                    }
                }
                else
                {
                    BallInPlay++;
                }

                LogInfo(nameof(PinGodGame), ":ball in play " + BallInPlay);
                GameData.BallsPlayed++;
                if (BallInPlay > BallsPerGame)
                {
                    LogDebug(nameof(PinGodGame), ": sending game ended with ball ended");
                    this.EmitSignal(nameof(BallEnded), true);
                    return true;
                }
                else
                {
                    LogDebug(nameof(PinGodGame), ": sending ball ended");
                    this.EmitSignal(nameof(BallEnded), false);
                }
            }
        }

        return false;
    }
    public virtual void EndOfGame()
    {
        GameInPlay = false;
        IsTilted = false;
        BallInPlay = 0;
        GameData.GamesFinished++;
        ResetTilt();
        gameEndTime = Time.GetTicksMsec();
        GameData.TimePlayed = gameEndTime - gameStartTime;
        if (_trough != null)
            _trough.BallsLocked = 0;
        this.EmitSignal(nameof(GameEnded));
    }
    public virtual ulong GetLastSwitchChangedTime(string sw) => Machine.Switches[sw].TimeSinceChange();
    public virtual Resources GetResources() => GetNodeOrNull<Resources>("Resources");
    public virtual bool IsSwitchAction(string[] switchNames, InputEvent input)
    {
        for (int i = 0; i < switchNames.Length; i++)
        {
            if (SwitchActionOn(switchNames[i], input))
            {
                return true;
            }
        }

        return false;
    }
    public virtual bool IsSwitchEnabled(string swName) => Machine.Switches[swName].IsEnabled;
    public virtual void LoadDataFile() => GameData = GameData.Load();
    public virtual void LoadPatches()
    {
        try
        {
            LogDebug(nameof(PinGodGame), ":looking for game patches. res://patch/patch_{patchNum}.pck . From 1. patch_1.pck, patch_2.pck");
            int patchNum = 1;
            bool success;
            while (success = ProjectSettings.LoadResourcePack($"res://patch/patch_{patchNum}.pck"))
            {
                LogInfo(nameof(PinGodGame), $":patch {patchNum} loaded");
                patchNum++;
            }
        }
        catch (Exception ex)
        {
            LogError(ex.ToString());
        }
    }
    public virtual void LogDebug(params object[] what) => Logger.Debug(what);
    public virtual void LogError(string message = null, params object[] what) => Logger.Error(message, what);
    public virtual void LogInfo(params object[] what) => Logger.Info(what);
    public virtual void LogWarning(string message = null, params object[] what) => Logger.Warning(message, what);
    public virtual void OnBallDrained(SceneTree sceneTree, string group = "Mode", string method = "OnBallDrained") => sceneTree.CallGroup(group, method);
    public virtual void OnBallSaved(SceneTree sceneTree, string group = "Mode", string method = "OnBallSaved") => sceneTree.CallGroup(group, method);
    public virtual void OnBallStarted(SceneTree sceneTree, string group = "Mode", string method = "OnBallStarted") => sceneTree.CallGroup(group, method);
    public virtual void PlayMusic(string name, float pos = 0) => AudioManager.PlayMusic(name, pos);
    public virtual void PlaySfx(string name, string bus = "Sfx") => AudioManager?.PlaySfx(name, bus);
    public virtual void PlayVoice(string name, string bus = "Voice") => AudioManager.PlayVoice(name, bus);
    public virtual void Quit(bool saveData = true)
    {
        //send game window ended, not alive        
        Machine.SetCoil("alive", 0);
        Logger.Info(nameof(PinGodGame), ":sent game ended coil: alive 0");

        //return if we've already quit
        if (this.QuitRequested) return;
        this.QuitRequested = true;

        if (saveData)
        {
            SaveWindow();

            //save override.cfg when not running editor. TODO: why not, or why? can't remember
            //if (!OS.HasFeature("editor")) 
            //{
            //    //save an override.cfg in project
            //    var path = "res://override.cfg"; //TODO: make name for game?
            //    LogDebug("no working directory, saving override.cfg settings", path);
            //    ProjectSettings.SaveCustom(path);
            //}

            SaveGameData();
            SaveGameSettings();
        }

        LogInfo(nameof(PinGodGame), ": Quit: saved game");

        if (GetTree().Paused) { GetTree().Paused = false; }
    }
    public virtual int RandomNumber(int from, int to) => randomNumGenerator.RandiRange(from, to);
    public virtual void ResetTilt()
    {
        Tiltwarnings = 0;
        IsTilted = false;
    }
    public virtual void SaveGameData() => GameData.Save(GameData);
    public virtual void SaveGameSettings() => Adjustments.Save(Adjustments);
    public virtual void SaveRecording()
    {
        if (_recordPlayback == RecordPlaybackOption.Record)
        {
            _recordFile?.Flush();
            _recordFile?.Free();
            LogInfo(nameof(PinGodGame), ":recording file closed");
        }
    }
    public virtual void SaveWindow()
    {
        var size = this.WinGetSize();
        var pos = this.WinGetPos();

        if (Adjustments?.Display != null)
        {
            Adjustments.Display.X = pos.x;
            Adjustments.Display.Y = pos.y;
            Adjustments.Display.Width = size.x;
            Adjustments.Display.Height = size.y;
        }
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.TEST_WIDTH, size.x);
        ProjectSettings.SetSetting(SettingPaths.DisplaySetPaths.TEST_HEIGHT, size.y);
    }
    public virtual void SetLampState(string name, byte state)
    {
        var lamp = Machine.SetLamp(name, state);
        if (_lampMatrixOverlay != null) { _lampMatrixOverlay.SetState(lamp.Num, state); }
    }
    public virtual void SetLedState(string name, byte state, int color = 0)
    {
        if (!LedExists(name)) return;
        var led = Machine.Leds[name];
        led.State = state;
        led.Color = color > 0 ? color : led.Color;
        if (_lampMatrixOverlay != null) { _lampMatrixOverlay.SetState(led.Num, state); }
    }
    public virtual void SetLedState(string name, byte state, System.Drawing.Color? colour = null)
    {
        if (!LedExists(name)) return;
        var c = colour.HasValue ?
            System.Drawing.ColorTranslator.ToOle(colour.Value) : Machine.Leds[name].Color;
        SetLedState(name, state, c);
    }
    public virtual void SetLedState(string name, byte state, Color? colour = null)
    {
        if (!LedExists(name)) return;
        var c = colour.HasValue ?
            System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(colour.Value.r8, colour.Value.g8, colour.Value.b8))
            : Machine.Leds[name].Color;
        SetLedState(name, state, c);
    }
    public virtual void SetLedState(string name, byte state, byte r, byte g, byte b)
    {
        if (!LedExists(name)) return;
        var c = System.Drawing.Color.FromArgb(r, g, b);
        var ole = System.Drawing.ColorTranslator.ToOle(c);
        SetLedState(name, state, ole);
    }
    public void SetSwitch(int swNum, byte value)
    {
        var sw = Machine.Switches.Values.FirstOrDefault(x => x.Num == swNum);
        if (sw != null)
        {
            SetSwitch(sw, value, false);
        }
    }
    public void SetSwitch(Switch @switch, byte value, bool fromAction = true)
    {
        if (!fromAction)
            @switch.SetSwitch(value > 0);

        //record switch
        if (_recordPlayback == RecordPlaybackOption.Record)
        {
            var switchTime = Time.GetTicksMsec() - gameLoadTimeMsec;
            var recordLine = $"sw{@switch.Num}|{true}|{switchTime}";
            _recordFile?.StoreLine(recordLine);
            LogDebug(nameof(PinGodGame), ":recorded:", recordLine);
        }

        //send SwitchCommand
        EmitSignal("SwitchCommand", @switch.Name, @switch.Num, value);
    }
    public virtual void Setup()
    {
        ServiceMenuEnter += OnServiceMenuEnter;

        //set up recording / playback
        SetUpRecordingsOrPlayback(_playback_game, _record_game, _playbackfile);

        //remove the overlays if disabled
        SetupDevOverlays();
    }
    public virtual void SetupDevOverlays()
    {
        //only try and find the 
        if (_lamp_overlay_enabled || _switch_overlay_enabled)
        {
            if (this.HasNode("DevOverlays"))
            {
                var devOverlays = GetNode("DevOverlays"); //will throw error
                if (devOverlays != null)
                {
                    LogDebug(nameof(PinGodGame), ":setting up overlays");
                    if (!_lamp_overlay_enabled)
                    {
                        LogDebug(nameof(PinGodGame), ":removing lamp overlay");
                        devOverlays.GetNode("LampMatrix").QueueFree();
                    }
                    else
                    {
                        _lampMatrixOverlay = devOverlays.GetNode("LampMatrix") as LampMatrix;
                    }

                    if (!_switch_overlay_enabled)
                    {
                        LogDebug(nameof(PinGodGame), ":removing switch overlay");
                        devOverlays.GetNode("SwitchOverlay").QueueFree();
                    }
                }
                else { LogDebug(nameof(PinGodGame), ":overlays disabled"); }
            }
            else
            {
                LogWarning("No DevOverlays node found");
            }
        }
    }
    public virtual void SetUpRecordingsOrPlayback(bool playbackEnabled, bool recordingEnabled, string playbackfile)
    {
        _recordPlayback = RecordPlaybackOption.Off;
        if (playbackEnabled) _recordPlayback = RecordPlaybackOption.Playback;
        else if (recordingEnabled) _recordPlayback = RecordPlaybackOption.Record;

        LogDebug(nameof(PinGodGame), ":setup playback?: ", _recordPlayback.ToString());
        if (_recordPlayback == RecordPlaybackOption.Playback)
        {
            if (string.IsNullOrWhiteSpace(playbackfile))
            {
                LogWarning("set a playback file from user directory eg: user://recordings/26232613.record");
                _recordPlayback = RecordPlaybackOption.Off;
            }
            else
            {
                LogInfo(nameof(PinGodGame), ":running playback file: ", playbackfile);
                try
                {
                    var pBackFile = FileAccess.Open(playbackfile, FileAccess.ModeFlags.Read);
                    if (FileAccess.GetOpenError() == Error.FileNotFound)
                    {
                        _recordPlayback = RecordPlaybackOption.Off;
                        LogError("playback file not found, set playback false");
                    }
                    else
                    {
                        string[] eventLine = null;
                        _playbackQueue = new Queue<PlaybackEvent>();
                        while ((eventLine = pBackFile.GetCsvLine("|"))?.Length == 3)
                        {
                            bool.TryParse(eventLine[1], out var state);
                            uint.TryParse(eventLine[2], out var time);
                            _playbackQueue.Enqueue(new PlaybackEvent(eventLine[0], state, time));
                        }
                        pBackFile.Free();
                        _playbackQueue.Reverse();
                        LogInfo(nameof(PinGodGame), $" {_playbackQueue.Count} playback events queued. first action: ", _playbackQueue.Peek().EvtName);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"playback file failed: " + ex.Message);
                }
            }
        }
        else if (_recordPlayback == RecordPlaybackOption.Record)
        {
            var userDir = CreateRecordingsDirectory();
            _recordFile = FileAccess.Open(playbackfile, FileAccess.ModeFlags.WriteRead);
            LogDebug(nameof(PinGodGame), ":game recording on");
        }
    }
    public virtual void SolenoidOn(string name, byte state) => Machine.SetCoil(name, state);
    public virtual async void SolenoidPulse(string name, byte pulse = 255)
    {
        if (!SolenoidExists(name)) return;

        var coil = Machine.Coils[name];
        await Task.Run(async () =>
        {
            coil.State = 1;
            await Task.Delay(pulse);
            coil.State = 0;
        });
    }
    public virtual void SolenoidPulseTimer(string name, float pulse = 0.255f)
    {
        if (!SolenoidExists(name)) return;

        var coil = Machine.Coils[name];

        var timer = new Timer() { Autostart = false, OneShot = true, Name = $"pulsetimer_{name}", WaitTime = pulse };
        //var arr = new Godot.Collections.Array(new Variant[] { name });
        //timer.Connect("timeout", this, nameof(OnSolenoidPulseTimeout), arr, (uint)ConnectFlags.Oneshot);

        //godot4. todo:check this is working
        timer.Connect("timeout", new Callable(this, nameof(OnSolenoidPulseTimeout)), (uint)ConnectFlags.OneShot);

        coil.State = 1;
        AddChild(timer);
        timer.Start();
    }
    public virtual bool StartGame()
    {
        LogInfo(nameof(PinGodGame), $":start game. BIP:{BallInPlay}, players/max:{Players.Count}/{MaxPlayers}, credits: {GameData.Credits}, inPlay:{GameInPlay}");
        if (IsTilted)
        {
            LogInfo(nameof(PinGodGame), ":Cannot start game when game is tilted");
            return false;
        }

        // first player start game
        if (!GameInPlay && GameData.Credits > 0)
        {
            LogInfo(nameof(PinGodGame), ":starting game, checking trough...");
            if (_trough != null)
            {
                if (!_trough.IsTroughFull()) //return if trough isn't full. TODO: needs debug option to remove check
                {
                    LogInfo(nameof(PinGodGame), ":Trough not ready. Can't start game with empty trough.");
                    PinGodMachine.BallSearchTimer.Start(1);
                    return false;
                }
            }
            else
            {
                Logger.Warning(nameof(PinGodGame), "start game, no trough found");
                return false;
            }

            Players.Clear(); //clear any players from previous game
            GameInPlay = true;
            if (PinGodMachine != null) PinGodMachine.GameInPlay = true;

            //remove a credit and add a new player
            GameData.Credits--;
            BallsPerGame = (byte)(Adjustments.BallsPerGame > 5 ? 5 : Adjustments.BallsPerGame);

            //TODO: set the ball save seconds from game settings
            //if(_trough != null)
            //    _trough._ball_save_seconds = (byte)(Adjustments.BallSaveTime > 20 ? 20 : Adjustments.BallSaveTime);

            CreatePlayer($"P{Players.Count + 1}");
            CurrentPlayerIndex = 0;
            Player = Players[CurrentPlayerIndex];
            LogDebug(nameof(PinGodGame), ":signal: player 1 added");
            GameData.GamesStarted++;
            gameStartTime = Time.GetTicksMsec();
            EmitSignal(nameof(PlayerAdded));
            EmitSignal(nameof(GameStarted));
            return true;
        }
        //game started already, add more players until max
        else if (BallInPlay <= 1 && GameInPlay && Players.Count < MaxPlayers && GameData.Credits > 0)
        {
            GameData.Credits--;
            CreatePlayer($"P{Players.Count + 1}");
            LogDebug(nameof(PinGodGame), $":signal: player added. {Players.Count}");
            EmitSignal(nameof(PlayerAdded));
            return true;
        }

        LogInfo(nameof(PinGodGame), ": start game, nothing happened.");
        return false;
    }
    public virtual void StartMultiBall(byte numOfBalls, byte ballSaveTime = 20, float pulseTime = 0)
    {
        if (_trough != null)
        {
            _trough.StartMultiball(numOfBalls, ballSaveTime, pulseTime);
        }

        IsMultiballRunning = true;
        EmitSignal(nameof(MultiballStarted));
    }
    public virtual void StartNewBall()
    {
        IsBallStarted = true;
        LogInfo(nameof(PinGodGame), ":starting new ball");
        GameData.BallsStarted++;
        ResetTilt();
        Player = Players[CurrentPlayerIndex];
        if (Player.ExtraBalls > 0 && Player.ExtraBallsAwarded < Adjustments.MaxExtraBalls)
        {
            Player.ExtraBalls--;
            Player.ExtraBallsAwarded++;
            LogInfo(nameof(PinGodGame), ": player shoot again");
        }

        if (_trough != null)
            _trough?.PulseTrough();
        else
            SolenoidPulse("trough");

        EnableFlippers(true);
    }
    public virtual float StopMusic() => AudioManager?.StopMusic() ?? -1;
    public virtual bool SwitchActionOff(string swName, InputEvent inputEvent)
    {
        if (!SwitchExists(swName)) return false;
        var sw = Machine.Switches[swName];
        var result = sw.IsActionOff(inputEvent);
        if (result)
        {
            SetSwitch(sw, 0);
        }
        return result;
    }
    public virtual bool SwitchActionOn(string swName, InputEvent inputEvent)
    {
        if (!SwitchExists(swName)) return false;
        var sw = Machine.Switches[swName];
        var result = sw.IsActionOn(inputEvent);
        if (result)
        {
            SetSwitch(sw, 1);
        }
        return result;
    }
    public virtual bool SwitchOn(string swName)
    {
        if (!SwitchExists(swName)) return false;
        return Machine.Switches[swName].IsActionOn();
    }
    public virtual void UpdateLamps(SceneTree sceneTree, string group = "Mode", string method = "UpdateLamps") => sceneTree.CallGroup(group, method); 
    #endregion

    #region Private_Methods
    protected virtual void SetupAudio()
    {
        LogInfo(nameof(PinGodGame), ":setting up audio from settings.save");
        if (this.HasNode(nameof(AudioManager)))
        {
            AudioManager = GetNode<AudioManager>("AudioManager");
            AudioServer.SetBusVolumeDb(0, Adjustments?.MasterVolume ?? 0f);
            AudioServer.SetBusVolumeDb(1, Adjustments?.MusicVolume ?? -6.0f);
            AudioServer.SetBusVolumeDb(2, Adjustments?.SfxVolume ?? -6.0f);
            AudioServer.SetBusVolumeDb(3, Adjustments?.VoiceVolume ?? -6.0f);
            AudioManager.MusicEnabled = Adjustments?.MusicEnabled ?? true;
            AudioManager.SfxEnabled = Adjustments?.SfxEnabled ?? true;
            AudioManager.VoiceEnabled = Adjustments?.VoiceEnabled ?? true;
        }
        else { LogWarning(nameof(PinGodGame), ": AudioManager node not found. Add an AudioManager child instance to the scene"); }
    }
    /// <summary>
    /// Creates the recordings directory in the users folder
    /// </summary>
    /// <returns>The path to the recordings</returns>
    private string CreateRecordingsDirectory()
    {
        var userDir = OS.GetUserDataDir();
        var dir = userDir + $"/recordings/";

        if (!System.IO.Directory.Exists(userDir))
            System.IO.Directory.CreateDirectory(dir);

        return dir;
    }
    /// <summary>
    /// Parses user command lines args in the --key=value format
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, string> GetCommandLineArgs()
    {
        var cmd = OS.GetCmdlineArgs();
        LogInfo(nameof(PinGodGame), ":cmd line available: ", cmd?.Length);
        Dictionary<string, string> _args = new Dictionary<string, string>();
        _args.Add("base_path", OS.GetExecutablePath());
        foreach (var arg in cmd)
        {
            if (arg.Contains("="))
            {
                var keyValue = arg.Split("=");
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].TrimStart('-', '-');
                    if (!_args.ContainsKey(key))
                    {
                        _args.Add(key, keyValue[1]);
                    }
                }
            }
        }

        return _args;
    }
    private bool LampExists(string name)
    {
        if (!Machine.Lamps.ContainsKey(name))
        {
            LogError($"ERROR:no lamp found for: {name}");
            return false;
        }

        return true;
    }
    private bool LedExists(string name)
    {
        if (!Machine.Leds.ContainsKey(name))
        {
            LogError($"ERROR:no led found for: {name}");
            return false;
        }

        return true;
    }
    /// <summary>
    /// Stops any game in progress
    /// </summary>
    private void OnServiceMenuEnter()
    {
        GameInPlay = false;
        if (PinGodMachine != null) PinGodMachine.GameInPlay = false;
        ResetTilt();
        EnableFlippers(false);
    }
    /// <summary>
    /// Queue frees a `pulsetimer_name` solenoid Timer from using <see cref="Machine.Coils"/>
    /// </summary>
    /// <param name="name"></param>
    private void OnSolenoidPulseTimeout(string name)
    {
        var pinObj = Machine.Coils[name];
        if (pinObj != null)
        {
            pinObj.State = 0;
            var timer = GetNodeOrNull<Timer>($"pulsetimer_{name}");
            Logger.Verbose(nameof(PinGodGame), $":pulsetimer_{name}:pulsed timed out");
            timer?.QueueFree();
        }
    }
    private void PinGodMachine_SwitchCommand(string name, byte index, byte value)
    {
        if (value > 0)
        {
            switch (index)
            {
                case 1:
                case 2:
                case 3: //Coin buttons. See PinGod.vbs for Standard switches
                    AudioManager?.PlaySfx("credit");
                    AddCredits((byte)(1 * index));
                    break;
                case 19://start
                    CallDeferred(nameof(StartGame));
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// Sets up window from settings. Sets position. Sets on top (the project should have this off in the Godot UI for this to work). Sets full screen
    /// </summary>
    private void SetupWindow()
    {
        if (Adjustments?.Display == null)
        {
            LogInfo(nameof(PinGodGame), ":creating initial display settings");
            if (Adjustments == null)
                Adjustments = new Adjustments();
            Adjustments.Display = new DisplaySettings() { AlwaysOnTop = true };
            SaveWindow();
        }

        GetTree().Root.ContentScaleMode = Adjustments.Display.ContentScaleMode;
        GetTree().Root.ContentScaleAspect = (Window.ContentScaleAspectEnum)Adjustments.Display.AspectOption;

        //on top
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.AlwaysOnTop, Adjustments.Display.AlwaysOnTop);
        //OS.MoveWindowToForeground();        

        //set the width, position
        DisplayServer.WindowSetPosition(new Vector2i((int)Adjustments.Display.X, (int)Adjustments.Display.Y));
        DisplayServer.WindowSetSize(new Vector2i((int)Adjustments.Display.Width, (int)Adjustments.Display.Height));
        var w = ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.WIDTH);
        var h = ProjectSettings.GetSetting(SettingPaths.DisplaySetPaths.HEIGHT);
        var size = DisplayServer.WindowGetSize();
        var pos = DisplayServer.WindowGetPosition();
        LogInfo(nameof(PinGodGame), $":window: size:{size.x}x{size.y} pos:{pos.x},{pos.y}, onTop: {Adjustments.Display.AlwaysOnTop}");
        LogInfo(nameof(PinGodGame), $":window: project settings size: ", $"{w}x{h}");

        //full screen        
        if (Adjustments.Display.FullScreen)
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);

        //title
        //DisplayServer.WindowSetTitle
    }
    private bool SolenoidExists(string name)
    {
        if (!Machine.Coils.ContainsKey(name))
        {
            LogError(nameof(SolenoidExists) + $" ERROR:no solenoid found: {name} \n");
            return false;
        }

        return true;
    }
    private bool SwitchExists(string name)
    {
        if (!Machine.Switches.ContainsKey(name))
        {
            LogError(nameof(SwitchExists) + $" :ERROR:no switch found: {name} \n");
            return false;
        }

        return true;
    } 
    #endregion
}
