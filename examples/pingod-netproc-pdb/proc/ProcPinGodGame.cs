using NetProc.Domain;
using PinGod.Core;
using PinGod.Game;
using System.Threading.Tasks;
using System.Threading;
using Godot;
using PinGod.Core.Service;
using NetProc.Game.Modes;
using System.Linq;

/// <summary>
/// Inheriting PinGodGame to take over with a P-ROC.
/// </summary>
public partial class ProcPinGodGame : PinGodGame
{
    private MachinePROC _procMachine;
    private CancellationTokenSource tokenSource;
    private Task _procGameLoop;

    /// <summary>
    /// Procgame <see cref="IGameController"/>
    /// </summary>
    public PinGodProcGameController _procGame;
    private PinGodProcMode _mode;
    private AttractMode _AttractMode;
    private Resources _resources;

    public override void _EnterTree()
    {
        base._EnterTree();
    }

    /// <summary>
    /// Quit the P-ROC game loop
    /// </summary>
    public override void _ExitTree()
    {        
        if (!tokenSource?.IsCancellationRequested ?? true)
            tokenSource?.Cancel();

        base._ExitTree();
    }

    /// <summary>
    /// Creates a P-ROC game then Starts the <see cref="IGameController.RunLoop"/>
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        _procMachine = this.MachineNode as MachinePROC;
        if (_procMachine != null)
        {

            _resources = GetResources();


            //CREATE AND SETUP PROC MACHINE
            CreateProcGame(_procMachine._machineConfig);
            Logger.Info(nameof(MachinePROC), ": ProcGame created. Setting up MachineNode from ProcGame.");


            //_procGame.Logger. = NetProc.Domain.PinProc.LogLevel.Verbose;
            Logger.LogLevel = PinGod.Base.LogLevel.Verbose;

            //SET MACHINE ITEMS FROM PROC TO PINGOD
            SetupPinGodotFromProcGame();
            Logger.Info(nameof(MachinePROC), ": ProcGame and database setup complete.");

            try 
            {
                StartProcGameLoop();
            }
            catch (System.Exception ex) { Logger.Error(nameof(MachinePROC), nameof(_Ready), $"{ex.Message} - {ex.InnerException?.Message}"); }
        }        
    }

    /// <summary>
    /// Runs until the resources are ready. For first run, load all scenes, this waits not blocking, then load the Attract.
    /// </summary>
    /// <param name="_delta"></param>
    public override void _Process(double _delta)
    {
        base._Process(_delta);
        if(_resources != null)
        {
            bool result = _resources?.IsLoading() ?? true;
            if (!result)
            {
                //resources loaded
                SetProcess(false);
                OnResourcesLoaded();
            }
        }
    }

    /// <summary>
    /// The Resources node has loaded all resources. Now we can get any packed scenes and use in modes <para/>
    /// </summary>
    private void OnResourcesLoaded()
    {
        _mode = new MyMode(_procGame, 10, this);
        _AttractMode = new AttractMode(_procGame, 12, this);

        //get the trough switches
        var troughSw = _procGame.Switches.Values
            .Where(x => x.Name.Contains("trough"))
            .Select(x=>x.Name);        
        var _troughMode = new Trough(_procGame, troughSw.ToArray(),
            "", "trough", new string[] { "outlaneL", "outlaneR" },
            "plungerLane");

        _troughMode.Game.Modes.Add(_troughMode);

        _troughMode.EnableBallSave(true);

        var ms = GetNodeOrNull<Node2D>("/root/ProcScene");
        if (ms != null)
        {
            //ms.AddChild(_mode); //TODO: addchild node
            _procGame.Modes.Add(_mode);
            _procGame.Modes.Add(_AttractMode);

            Logger.Info($"MODES RUNNING:" + _procGame.Modes.Modes.Count);
            _procGame.AddPlayer();
        }
    }

    /// <summary>
    /// Must set solution / project to x86 if running real p-roc board
    /// true here is simulated and can get away with running with FakePinProc            
    /// </summary>
    /// <param name="machineConfig"></param>
    private void CreateProcGame(MachineConfiguration machineConfig)
    {
        _procGame = new PinGodProcGameController(machineConfig.PRGame.MachineType, new PinGodProcLogger(), true);

        //load config and setup machine
        _procGame.LoadConfig(machineConfig);
    }

    /// <summary>
    /// Adds machine items from <see cref="_procGame"/> into this Machine node <para/>
    /// TODO: perhaps this machine class overrides the base and uses the <see cref="_procGame"/> instead
    /// </summary>
    private void SetupPinGodotFromProcGame()
    {
        _procMachine.ClearMachineItems();

        if (_procGame != null)
        {
            //add switches from p-roc
            if (_procGame.Switches?.Count > 0)
                _procGame.Switches.Values.ForEach(x => _procMachine.AddSwitch(x.Name, (byte)x.Number));
            if (_procGame.Coils?.Count > 0)
                _procGame.Coils.Values.ForEach(x => _procMachine.AddCoil(x.Name, (byte)x.Number));
            if (_procGame.Lamps?.Count > 0)
                _procGame.Lamps.Values.ForEach(x => _procMachine.AddLamp(x.Name, (byte)x.Number));
            if (_procGame.LEDS?.Count > 0)
                _procGame.LEDS.Values.ForEach(x => _procMachine.AddLed(x.Name, (byte)x.Number));
        }
    }

    /// <summary>
    /// Starts netproc <see cref="IGameController.RunLoop"/>, creates cancel token source to end the loop
    /// </summary>
    private void StartProcGameLoop()
    {
        if (_procGameLoop?.Status == TaskStatus.Running) { return; }

        tokenSource = new CancellationTokenSource();
        _procGameLoop = Task.Run(() =>
        {
            Logger.Info(nameof(MachinePROC), ":running game loop");

            //run proc game loop delay 1 save CPU
            _procGame.RunLoop(1, tokenSource);

            Logger.Info(nameof(MachinePROC), ":ending proc game loop");
            _procGame.EndRunLoop();
            Logger.Info(nameof(MachinePROC), ":proc game loop stopped");

        }, tokenSource.Token);
    }
}
