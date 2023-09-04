using Godot;
using NetProc.Data;
using NetProc.Data.Model;
using NetProc.Domain;
using NetProc.Domain.Modes;
using NetProc.Domain.PinProc;
using NetProc.Game;
using PinGod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
/// A PinGod P-ROC Game controller. This overrides methods from the P-ROC <see cref="BaseGameController"/>. <para/>
/// This GameController is built using an <see cref="IFakeProcDevice"/>, but that could be a real* <see cref="IProcDevice"/> board by switching off simulation. *The project and Godot would have to be x86 if real <para/>
/// Add modes to <see cref="BallStarting"/> and remove them from <see cref="BallEnded"/> (this is if the mode starts and ends here of course) <para/>
/// <see cref="GameStarted"/> adds the ScoreDisplay. (Attract is removed from switch handler in that mode). <para/>
/// <see cref="GameEnded"/> removes the ScoreDisplay and adds the attract (which is newed up)
/// </summary>
public class PinGodProcGameController : NetProcDataGameController
{
    public readonly PinGodGameProc PinGodGame;
    public readonly IFakeProcDevice ProcFake;

    public byte[] _lastCoilStates;

    public int[] _lastLedStates;

    /// <summary>
    /// A P-ROC based <see cref="IMode"/>
    /// </summary>
    public AttractMode _AttractMode;
    private BallSave _ballSave;
    private BallSearch _ballSearch;
    private ServiceMode _serviceMode;
    private CancellationTokenSource _gameLoopCancelToken;
    private MachineSwitchHandlerMode _machineSwitchHandlerMode;
    private MemoryMapPROCNode _memMap;
    private MyMode _myMode;
    private ScoreDisplayProcMode _scoreDisplay;
    private bool _isSimulated;

    public PinGodProcGameController(MachineType machineType, bool deleteOnInit = false, ILogger logger = null, bool simulated = false,
        IPinGodGame pinGodGame = null, MachineConfiguration configuration = null) 
        : base(machineType, deleteOnInit, logger, simulated, configuration)
    {
        ProcFake = PROC as IFakeProcDevice;
        PinGodGame = pinGodGame as PinGodGameProc;
        PinGodGame.Credits = GetAudit("CREDITS");
        _isSimulated = simulated;
    }

    /// <summary>
    /// Use the godot call deffered to use this
    /// </summary>
    /// <param name="modeName"></param>
    public void AddMode(string modeName)
    {
        switch (modeName)
        {
            case "service":
                Modes.Add(_serviceMode);
                break;
            case "attract":                
                MachineResourcesReady();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Use the godot call deffered to use this
    /// </summary>
    /// <param name="modeName"></param>
    public void RemoveMode(string modeName)
    {
        switch (modeName)
        {
            case "service":
                Modes.Remove(_serviceMode);
                break;
            case "attract":
                Modes.Remove(_AttractMode);
                break;
            default:
                break;
        }
    }

    public override IPlayer AddPlayer()
    {
        //todo: name the player when need to change from eg: Player 1
        var p = base.AddPlayer();
        p = new CustomPlayer(p.Name);
        _scoreDisplay?.UpdateScores();        
        return p;
    }

    /// <summary>
    /// Add points to the CurrentPlayer and update the Godot display
    /// </summary>
    /// <param name="p">points</param>
    public override void AddPoints(long p)
    {
        base.AddPoints(p);
        _scoreDisplay?.UpdateScores();
    }

    public override void BallEnded()
    {
        base.BallEnded();

        FlippersEnabled = false;
        if (_isSimulated) Coils["flippersRelay"].Disable();

        //TODO: remove modes on ball starting, these modes remove when a new ball ends
        Modes.Remove(_myMode);
        _scoreDisplay?.UpdateScores();

        //add ball stats for the player
        var player = CurrentPlayer() as PlayerDb;
        player.BallStats.Add(new BallPlayed { Ball = (byte)this.Ball, Points = player.Score, Time = this.GetBallTime() });        
    }

    public override void BallStarting()
    {
        base.BallStarting();
        //TODO: add modes on ball starting, these modes start when a new ball does
        FlippersEnabled = true;
        if(_isSimulated) Coils["flippersRelay"].Enable();

        _myMode = new MyMode(this, 10, (PinGodGameProc)PinGodGame);
        Modes.Add(_myMode);
        _scoreDisplay?.UpdateScores();
        _ballSave.Start(now: false);
    }

    public override void GameEnded()
    {
        //this.EmitSignal(nameof(GameEnded));

        base.GameEnded();
        Modes.Remove(_scoreDisplay);
        this.MachineResourcesReady();
    }

    public override void GameStarted()
    {
        base.GameStarted();        
        //_scoreDisplay = new ScoreDisplayProcMode(this, (PinGodGameProc)PinGodGame);
        Modes.Add(_scoreDisplay);
    }

    /// <summary>
    /// Used by simulators. Returns the led colors / states as int array. Used with memory mapping in loop
    /// </summary>
    /// <param name="stateCount"></param>
    /// <returns></returns>
    public int[] GetLedStatesArray(IEnumerable<LED> leds)
    {
        if (_memMap == null) return null;
        int[] arr = new int[_memMap.LedCount()];
        foreach (var item in leds)
        {
            if (item.Number * 3 <= arr.Length)
            {
                var c = System.Drawing.Color.FromArgb((int)item.CurrentColor[0], (int)item.CurrentColor[1], (int)item.CurrentColor[2]);
                arr[item.Number * 3] = item.Number;
                arr[item.Number * 3 + 1] = 1;
                arr[item.Number * 3 + 2] = System.Drawing.ColorTranslator.ToOle(c);
            }
        }

        return arr;
    }

    /// <summary>
    /// <inheritdoc/> <para/> Increments the POWERED_ON_TIMES.
    /// </summary>
    public override void InitializeDatabase()
    {
        var initTime = Godot.Time.GetTicksMsec();

        base.InitializeDatabase();        

        var endTime = Godot.Time.GetTicksMsec();
        var total = (endTime - initTime) / 1000;

        Logger.Log(nameof(PinGodProcGameController) + $": database initialized in {total} secs.");
    }

    /// <summary>
    /// Let base deal with this to EndBall when needed
    /// </summary>
    public override void OnBallDrainedTrough() => base.OnBallDrainedTrough();

    /// <summary>
    /// Add the modes on game reset?
    /// </summary>
    public override void Reset()
    {
        base.Reset();

        //_troughMode.EnableBallSave(true);        

        //link the trough to ball save
        Trough.BallSaveCallback = new AnonDelayedHandler(_ballSave.LaunchCallback);
        Trough.NumBallsToSaveCallback = new GetNumBallsToSaveHandler(_ballSave.GetNumBallsToSave);
        _ballSave.TroughEnableBallSave = new BallSaveEnable(Trough.EnableBallSave);

        Modes.Add(_AttractMode);
        Modes.Add(_machineSwitchHandlerMode);
        Modes.Add(_ballSave);
        Modes.Add(_ballSearch);

        Logger.Log($"MODES RUNNING:" + Modes.Modes.Count);
    }

    public override void RunLoop(byte delay = 0, CancellationTokenSource cancellationToken = null)
    {
        //base.RunLoop(delay, cancellationToken);
        //return;

        if (cancellationToken == null)
            _gameLoopCancelToken = new CancellationTokenSource();
        else
            _gameLoopCancelToken = cancellationToken;

        if (_isSimulated)
        {
            var node = PinGodGame.CallDeferred("get_node_or_null", "/root/MemoryMap");
            _memMap = node.As<MemoryMapPROCNode>();

            if (_memMap == null)
                Logger.Log(nameof(PinGodProcGameController), $": WARN: no {nameof(MemoryMapPROC)} found in root/MemoryMap");
            _lastCoilStates = GetStates(Coils.Values);
            _lastLedStates = GetLedStatesArray(LEDS.Values);
            _memMap?.WriteStates();
        }        

        Event[] events;
        try
        {
            while (!_gameLoopCancelToken.IsCancellationRequested)
            {
                events = GetEvents(false);
                if (events?.Count() > 0)
                {
                    foreach (var evt in events) { ProcessEvent(evt);}
                }

                this.Tick();
                //TickVirtualDrivers();                

                //PinGod changed states. Used by memory mapping
                if (_isSimulated)
                {
                    _lastCoilStates = GetStates(Coils.Values);
                    _lastLedStates = GetLedStatesArray(LEDS.Values);
                    _memMap?.WriteStates();
                }

                foreach (var coil in _coils.Values)
                {
                    ((IVirtualDriver)coil).Tick();
                }
                foreach (var led in _leds.Values)
                {
                    led.Tick();
                }

                //MODES: Tick
                _modes?.Tick();

                //_proc?.WatchDogTickle();
                if (delay > 0)
                    Thread.Sleep(delay);
            }
        }
        catch (Exception ex)
        {
            Logger.Log("RUN LOOP EXCEPTION: " + ex.ToString(), LogLevel.Error);
        }
        finally
        {
            Logger?.Log("Run loop ended", LogLevel.Info);
            double dt = NetProc.Domain.Time.GetTime() - BootTime;
            _proc.Close();
        }
    }

    public override void ShootAgain()
    {
        base.ShootAgain();
        Logger.Log($"extra balls: {this.CurrentPlayer().ExtraBalls}", LogLevel.Debug);
    }

    public override void StartBall() => base.StartBall();

    public override void StartGame() => base.StartGame();

    internal void BallSearch() => _ballSearch?.PerformSearch();

    /// <summary>
    /// Saves the database
    /// </summary>
    internal void ExitGame()
    {
        try
        {
            Quit();
        }
        catch (System.Exception ex)
        {
            Logger.Log(nameof(PinGodProcGameController) + $"--ERROR: {ex.Message} {ex.InnerException?.Message}", LogLevel.Error);
        }
    }    

    /// <summary>
    /// After display has loaded all the 'first load' resources. This calls reset on the game.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    internal void MachineResourcesReady()
    {
        _AttractMode = new AttractMode(this, 12, PinGodGame);
        _serviceMode = new ServiceMode(this, PinGodGame, priority: 10, defaultScene: "res://scenes/ServiceMode/ServiceModePROC.tscn".GetBaseName());
        _scoreDisplay = new ScoreDisplayProcMode(this, (PinGodGameProc)PinGodGame, priority: 2);
        _machineSwitchHandlerMode = new MachineSwitchHandlerMode(this, (PinGodGameProc)PinGodGame);
        _ballSave = new BallSave(this, "shootAgain", "plungerLane") { AllowMultipleSaves = false, Priority = 25 };
        SetupBallSearch();
        Reset();
    }

    byte[] GetStates(IEnumerable<IDriver> drivers)
    {
        if (_memMap == null) return null;

        byte[] arr = new byte[_memMap.CoilCount()];
        foreach (var item in drivers)
        {
            var num = (byte)(item.Number - 40);
            if (num * 2 <= arr.Length)
            {
                arr[num * 2] = num;
                arr[num * 2 + 1] = ((IVirtualDriver)item).GetCurrentState() ? (byte)1 : (byte)0;
            }
        }
        return arr;
    }

    private void SetupBallSearch()
    {
        var coils = Config.PRCoils.Where(x => x.Search > 0)
                    .Select(n => n.Name)
                    .ToArray();

        var resetDict = new Dictionary<string, string>();
        var resetSw = Config.PRSwitches
            .Where(x => string.IsNullOrWhiteSpace(x.SearchReset));
        foreach (var sw in resetSw)
        {
            resetDict.Add(sw.Name, sw.SearchReset);
        }

        var stopDict = new Dictionary<string, string>();
        var stopSw = Config.PRSwitches
            .Where(x => string.IsNullOrWhiteSpace(x.SearchStop));
        foreach (var sw in stopSw)
        {
            stopDict.Add(sw.Name, sw.SearchStop);
        }

        _ballSearch = new BallSearch(this, 12, coils, resetDict, stopDict, null);
    }
}
