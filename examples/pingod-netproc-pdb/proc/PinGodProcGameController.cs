using NetProc.Domain;
using NetProc.Domain.PinProc;
using NetProc.Game;
using PinGod.Core;
using System;

/// <summary>
/// A PinGod P-ROC Game controller. This overrides methods from the P-ROC <see cref="BaseGameController"/>. <para/>
/// This GameController is built using an <see cref="IFakeProcDevice"/>, but that could be a real* <see cref="IProcDevice"/> board by switching off simulation. *The project and Godot would have to be x86 if real <para/>
/// Add modes to <see cref="BallStarting"/> and remove them from <see cref="BallEnded"/> (this is if the mode starts and ends here of course) <para/>
/// <see cref="GameStarted"/> adds the ScoreDisplay. (Attract is removed from switch handler in that mode). <para/>
/// <see cref="GameEnded"/> removes the ScoreDisplay and adds the attract (which is newed up)
/// </summary>
public class PinGodProcGameController : BaseGameController
{
    public readonly IPinGodGame PinGodGame;
    public readonly IFakeProcDevice ProcFake;

    /// <summary>
    /// A P-ROC based <see cref="IMode"/>
    /// </summary>
    private AttractMode _AttractMode;
    private PinGodProcMode _mode;
    private ScoreDisplayProcMode _scoreDisplay;
    private MachineSwitchHandlerMode _machineSwitchHandlerMode;

    public PinGodProcGameController(MachineType machineType, ILogger logger = null, bool simulated = false,
        MachineConfiguration configuration = null, IPinGodGame pinGodGame = null)
        : base(machineType, logger, simulated, configuration)
    {
        ProcFake = PROC as IFakeProcDevice;
        PinGodGame = pinGodGame;
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
        //TODO: remove modes on ball starting, these modes remove when a new ball ends
        Modes.Remove(_mode);
    }

    public override void BallStarting()
    {
        base.BallStarting();
        //TODO: add modes on ball starting, these modes start when a new ball does
        _mode = new MyMode(this, 10, (PinGodGameProc)PinGodGame);
        Modes.Add(_mode);
    }

    public override void GameEnded()
    {
        base.GameEnded();
        Modes.Remove(_scoreDisplay);

        _machineSwitchHandlerMode = new MachineSwitchHandlerMode(this, (PinGodGameProc)PinGodGame);
        _AttractMode = new AttractMode(this, 12, PinGodGame);
        Modes.Add(_machineSwitchHandlerMode);
        Modes.Add(_AttractMode);
    }

    public override void GameStarted()
    {
        base.GameStarted();
        _scoreDisplay = new ScoreDisplayProcMode(this, 2, (PinGodGameProc)PinGodGame);
        Modes.Add(_scoreDisplay);
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

        Modes.Add(_machineSwitchHandlerMode);
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
        Reset();
    }
}
