using NetProc.Data;
using NetProc.Data.Model;
using NetProc.Domain;
using NetProc.Domain.Modes;
using NetProc.Domain.PinProc;
using NetProc.Domain.Players;
using NetProc.Game;
using PinGod.Core;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A PinGod P-ROC Game controller. This overrides methods from the P-ROC <see cref="BaseGameController"/>. <para/>
/// This GameController is built using an <see cref="IFakeProcDevice"/>, but that could be a real* <see cref="IProcDevice"/> board by switching off simulation. *The project and Godot would have to be x86 if real <para/>
/// Add modes to <see cref="BallStarting"/> and remove them from <see cref="BallEnded"/> (this is if the mode starts and ends here of course) <para/>
/// <see cref="GameStarted"/> adds the ScoreDisplay. (Attract is removed from switch handler in that mode). <para/>
/// <see cref="GameEnded"/> removes the ScoreDisplay and adds the attract (which is newed up)
/// </summary>
public class PinGodProcGameController : BaseGameController
{
    /// <summary>
    /// Delete the database each time game is run? Useful when adding new items when testing
    /// </summary>
    const bool DELETE_DB_ON_INIT = true;
    const bool IS_PDB_MACHINE = true;

    public readonly PinGodGameProc PinGodGame;
    public readonly IFakeProcDevice ProcFake;
    public INetProcDbContext Database;

    /// <summary>
    /// A P-ROC based <see cref="IMode"/>
    /// </summary>
    private AttractMode _AttractMode;
    private BallSave _ballSave;
    private MyMode _myMode;
    private ScoreDisplayProcMode _scoreDisplay;
    private MachineSwitchHandlerMode _machineSwitchHandlerMode;
    private BallSearch _ballSearch;
    private List<NetProc.Data.Model.Player> _dbPlayers = new List<NetProc.Data.Model.Player>();
    GamePlayed _gamePlayed = new GamePlayed();

    public PinGodProcGameController(MachineType machineType, ILogger logger = null, bool simulated = false,
        MachineConfiguration configuration = null, IPinGodGame pinGodGame = null)
        : base(machineType, logger, simulated, configuration)
    {
        ProcFake = PROC as IFakeProcDevice;
        PinGodGame = pinGodGame as PinGodGameProc;

        try
        {
            InitDatabaseAndMachineConfig();
            PinGodGame.Credits = Database.GetAuditValue("CREDITS");
        }
        catch (System.Exception ex)
        {
            Logger.Log(nameof(MachinePROC) + $"{ex.Message} {ex.InnerException?.Message}", LogLevel.Error);
            throw;
        }
    }

    public override IPlayer AddPlayer()
    {
        //todo: name the player when need to change from eg: Player 1
        var p = base.AddPlayer();
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

    public override IPlayer CreatePlayer(string name)
    {
        var dbPlayer = Database.Players.FirstOrDefault(p => p.Name == name);
        if (dbPlayer == null)
        {
            dbPlayer = new NetProc.Data.Model.Player() { Name = name, Initials = "" };
            Database.Players.Add(dbPlayer);
            Database.SaveChanges();
            Logger.Log(nameof(PinGodProcGameController) + $"created new database player");
        }
        _dbPlayers.Add(dbPlayer);

        var player = new PinGodPROCPlayer(name, dbPlayer.Id);
        return player;
    }

    public override void BallEnded()
    {
        base.BallEnded();
        Database.IncrementAuditValue("TOTAL_BALLS_PLAYED", 1);
        //TODO: remove modes on ball starting, these modes remove when a new ball ends
        Modes.Remove(_myMode);
        _scoreDisplay?.UpdateScores();

        //add ball stats for the player
        var player = CurrentPlayer() as PinGodPROCPlayer;
        player.BallStats.Add(new BallPlayed { Ball = (byte)this.Ball, Points = player.Score, Time = this.GetBallTime() });
    }

    public override void BallStarting()
    {
        base.BallStarting();
        //TODO: add modes on ball starting, these modes start when a new ball does
        _myMode = new MyMode(this, 10, (PinGodGameProc)PinGodGame);
        Modes.Add(_myMode);
        _scoreDisplay?.UpdateScores();
        _ballSave.Start(now: false);
    }

    public override void GameEnded()
    {
        base.GameEnded();
        Modes.Remove(_scoreDisplay);
        SaveGamePlayed();

        _machineSwitchHandlerMode = new MachineSwitchHandlerMode(this, (PinGodGameProc)PinGodGame);
        _AttractMode = new AttractMode(this, 12, PinGodGame);
        Modes.Add(_machineSwitchHandlerMode);
        Modes.Add(_AttractMode);
    }

    private void SaveGamePlayed()
    {
        Database.IncrementAuditValue("GAMES_PLAYED", 1);

        //get game time for all players combined
        _gamePlayed.Ended = DateTime.Now;
        double totalTime = 0;
        for (int i = 0; i < Players.Count; i++) totalTime += GetGameTime(i);
        _gamePlayed.GameTime = totalTime;
        Database.GamesPlayed.Add(_gamePlayed);
        Database.SaveChanges();

        //each of our saved players has ball stats, add these to a ball played and the total score
        foreach (PinGodPROCPlayer p in Players)
        {
            var score = new Score()
            {
                Points = p.Score,
                PlayerId = p.Id,
                ExtraBallsPlayed = 0,
                GamePlayedId = _gamePlayed.Id
                //ExtraBallsPlayed //TODO
            };

            //add a ball played from the players ball stats
            p.BallStats.ForEach(x =>
            {
                Database.BallsPlayed.Add(new BallPlayed
                {
                    Ball = x.Ball,
                    Time = x.Time,
                    PlayerId = p.Id,
                    Points = x.Points,
                    Score=score
                });
            });

            Database.Scores.Add(score);
        }

        try
        {
            
            Database.SaveChanges();
        }
        catch (Exception ex)
        {
            Logger.Log(nameof(PinGodProcGameController) + $" database failed to save game played: {ex.Message} - {ex.InnerException?.Message}", LogLevel.Error);
        }
        _dbPlayers.Clear();
        _gamePlayed = new GamePlayed();
    }

    public override void GameStarted()
    {
        base.GameStarted();
        _gamePlayed.Started = DateTime.Now;
        Database.IncrementAuditValue("GAMES_STARTED", 1);
        _scoreDisplay = new ScoreDisplayProcMode(this, 2, (PinGodGameProc)PinGodGame);
        Modes.Add(_scoreDisplay);
    }

    /// <summary>
    /// Creates 
    /// </summary>
    /// <returns></returns>
    private void InitDatabaseAndMachineConfig()
    {
        var initTime = Godot.Time.GetTicksMsec();
        //DATABASE - EF CORE SQLITE
        Logger.Log(nameof(PinGodProcGameController) + ": init database");
        Database = new NetProcDbContext();
        Database.InitializeDatabase(IS_PDB_MACHINE, DELETE_DB_ON_INIT);

        //MACHINE CONFIG FROM DATABASE TABLES
        Logger.Log(nameof(PinGodProcGameController) + ": database init complete, creating" + nameof(MachineConfiguration));
        _config = Database.GetMachineConfiguration();
        Logger.Log(nameof(PinGodProcGameController) + ": machine config created\n",
            $"      Machine Type: {_config.PRGame.MachineType}, Balls: {_config.PRGame.NumBalls}\n Creating ProcGame...");
        this.LoadConfig(_config);

        Database.IncrementAuditValue("POWERED_ON_TIMES", 1);

        var endTime = Godot.Time.GetTicksMsec();
        var total = (endTime - initTime) / 1000;
        Logger.Log(nameof(PinGodProcGameController) + $": database initialized in {total} secs. {nameof(DELETE_DB_ON_INIT)}? {DELETE_DB_ON_INIT}");
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

        Modes.Add(_machineSwitchHandlerMode);
        Modes.Add(_ballSave);
        Modes.Add(_ballSearch);
        Modes.Add(_AttractMode);

        Logger.Log($"MODES RUNNING:" + Modes.Modes.Count);
        AddPlayer();
    }

    public override void ShootAgain()
    {
        base.ShootAgain();
        Logger.Log($"extra balls: {this.CurrentPlayer().ExtraBalls}", LogLevel.Debug);
    }

    public override void StartGame() => base.StartGame();
    public override void StartBall() => base.StartBall();

    /// <summary>
    /// After display has loaded all the 'first load' resources. This calls reset on the game.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    internal void MachineResourcesReady()
    {
        _AttractMode = new AttractMode(this, 12, PinGodGame);
        _scoreDisplay = new ScoreDisplayProcMode(this, 2, (PinGodGameProc)PinGodGame);
        _machineSwitchHandlerMode = new MachineSwitchHandlerMode(this, (PinGodGameProc)PinGodGame);
        _ballSave = new BallSave(this, "shootAgain", "plungerLane") { AllowMultipleSaves = false, Priority = 25 };
        SetupBallSearch();
        Reset();
    }

    private void SetupBallSearch()
    {
        var coils = Config.PRCoils.Where(x => x.Search > 0)
                    .Select(n => n.Name)
                    .ToArray();

        var resetDict = new System.Collections.Generic.Dictionary<string, string>();
        var resetSw = Config.PRSwitches
            .Where(x => string.IsNullOrWhiteSpace(x.SearchReset));
        foreach (var sw in resetSw)
        {
            resetDict.Add(sw.Name, sw.SearchReset);
        }

        var stopDict = new System.Collections.Generic.Dictionary<string, string>();
        var stopSw = Config.PRSwitches
            .Where(x => string.IsNullOrWhiteSpace(x.SearchStop));
        foreach (var sw in stopSw)
        {
            stopDict.Add(sw.Name, sw.SearchStop);
        }

        _ballSearch = new BallSearch(this, 12, coils, resetDict, stopDict, null);
    }

    internal void BallSearch()
    {
        _ballSearch?.PerformSearch();
    }

    internal void ExitGame()
    {
        if (Database != null)
        {
            try
            {
                //any changes to our lookup are tracked, this saves
                Database.SaveChanges();
                Database.Dispose();
            }
            catch (System.Exception ex)
            {
                Logger.Log(nameof(PinGodProcGameController) + $"--ERROR: {ex.Message} {ex.InnerException?.Message}", LogLevel.Error);
            }
        }
    }

}
