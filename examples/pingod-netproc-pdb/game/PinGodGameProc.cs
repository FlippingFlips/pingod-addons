using NetProc.Domain;
using PinGod.Core;
using PinGod.Game;
using System.Threading.Tasks;
using System.Threading;
using Godot;
using PinGod.Core.Service;
using System.Linq;
using NetProc.Data;

/// <summary>
/// Inheriting PinGodGame to take over with a P-ROC.
/// </summary>
public partial class PinGodGameProc : PinGodGame
{    
    public INetProcDbContext Database;
    /// <summary>
    /// Delete the database each time game is run? Useful when adding new items when testing
    /// </summary>
    const bool DELETE_DB_ON_INIT = false;
    const bool IS_PDB_MACHINE = true;
    const int PROC_DELAY = 1;

    /// <summary>
    /// Procgame <see cref="IGameController"/>
    /// </summary>
    public PinGodProcGameController PinGodProcGame;    
    private Task _procGameLoop;
    /// <summary>
    /// P-ROC version of the <see cref="MachineNode"/>. Get to the database throgh this
    /// </summary>
    public MachinePROC MachinePROC;
    /// <summary>
    /// Resources if found in root when the plug-in is enabled
    /// </summary>
    private Resources _resources;
    /// <summary>
    /// To cancel the PROC loop
    /// </summary>
    private CancellationTokenSource tokenSource;
    private MachineConfiguration _machineConfig;

    public PinGodGameProc()
    {
        try
        {
            InitDatabaseAndMachineConfig();
            Credits = Database.GetAuditValue("CREDITS");
        }
        catch (System.Exception ex)
        {
            Logger.Error(nameof(MachinePROC), $"{ex.Message} {ex.InnerException?.Message}");
            throw;
        }
    }

    #region Godot Overrides
    public override void _EnterTree()
    {
        base._EnterTree();
    }

    /// <summary>
    /// Quit the P-ROC game loop
    /// </summary>
    public override void _ExitTree()
    {
        if (Database != null)
        {
            try
            {
                //set display size + pos to database
                WindowSaveSettings();
                //any changes to our lookup are tracked, this saves
                Database.SaveChanges();
                Database.Dispose();
            }
            catch (System.Exception ex)
            {
                Logger.Error(nameof(MachinePROC), $"--ERROR: {ex.Message} {ex.InnerException?.Message}");
            }
        }

        if (!tokenSource?.IsCancellationRequested ?? true)
            tokenSource?.Cancel();

        base._ExitTree();
    }

    /// <summary>
    /// Runs until the resources are ready. For first run, load all scenes, this waits not blocking, then load the Attract.
    /// </summary>
    /// <param name="_delta"></param>
    public override void _Process(double _delta)
    {
        base._Process(_delta);
        if (_resources != null)
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
    /// Creates a P-ROC game then Starts the <see cref="IGameController.RunLoop"/>
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        MachinePROC = this.MachineNode as MachinePROC;
        if (MachinePROC != null)
        {
            WindowLoadSettings();

            _resources = GetResources();

            //CREATE AND SETUP PROC MACHINE
            CreateProcGame(_machineConfig);
            Logger.Info(nameof(MachinePROC), ": ProcGame created. Setting up MachineNode from ProcGame.");

            //PinGodProcGame.Logger. = NetProc.Domain.PinProc.LogLevel.Verbose;
            Logger.LogLevel = PinGod.Base.LogLevel.Verbose;

            //SET MACHINE ITEMS FROM PROC TO PINGOD
            SetupPinGodotFromProcGame();
            Logger.Info(nameof(MachinePROC), ": ProcGame and database setup complete.");
        }
    }
    #endregion

    public override void AddCredits(byte amt)
    {        
        Credits += amt;
        Database.IncrementAuditValue("CREDITS", amt);
        EmitSignal(nameof(CreditAdded), Credits);
    }

    /// <summary>
    /// Override the default window setup
    /// </summary>
    public override void SetupWindow()
    {
        //base.SetupWindow();        
    }

    /// <summary>
    /// Must set solution / project to x86 if running real p-roc board
    /// true here is simulated and can get away with running with FakePinProc            
    /// </summary>
    /// <param name="machineConfig"></param>
    private void CreateProcGame(MachineConfiguration machineConfig)
    {
        PinGodProcGame = new PinGodProcGameController(machineConfig.PRGame.MachineType, 
            new PinGodProcLogger() { LogLevel = NetProc.Domain.PinProc.LogLevel.Verbose}, true, machineConfig, this);
        //don't need to use LoadConfig anymore with this controller
        //PinGodProcGame.LoadConfig(machineConfig);
    }


    /// <summary>
    /// Creates 
    /// </summary>
    /// <returns></returns>
    private void InitDatabaseAndMachineConfig()
    {
        var initTime = Godot.Time.GetTicksMsec();
        //DATABASE - EF CORE SQLITE
        Logger.Info(nameof(MachinePROC), ": init database");
        Database = new NetProcDbContext();
        Database.InitializeDatabase(IS_PDB_MACHINE, DELETE_DB_ON_INIT);

        //MACHINE CONFIG FROM DATABASE TABLES
        Logger.Info(nameof(MachinePROC), ": database init complete, creating" + nameof(MachineConfiguration));
        _machineConfig = Database.GetMachineConfiguration();
        Logger.Info(nameof(MachinePROC), ": machine config created\n",
            $"      Machine Type: {_machineConfig.PRGame.MachineType}, Balls: {_machineConfig.PRGame.NumBalls}\n Creating ProcGame...");


        Database.IncrementAuditValue("POWERED_ON_TIMES", 1);

        var endTime = Godot.Time.GetTicksMsec();
        var total = (endTime - initTime) / 1000;
        Logger.Info(nameof(MachinePROC), $": database initialized in {total} secs. {nameof(DELETE_DB_ON_INIT)}? {DELETE_DB_ON_INIT}");
    }

    /// <summary>
    /// The Resources node has loaded all resources. Now we can get any packed scenes and use in modes <para/>
    /// </summary>
    private void OnResourcesLoaded()
    {
        var ms = GetNodeOrNull<Node2D>("/root/ProcScene");
        if (ms != null)
        {
            try
            {
                StartProcGameLoop();
            }
            catch (System.Exception ex) { Logger.Error(nameof(MachinePROC), nameof(_Ready), $"{ex.Message} - {ex.InnerException?.Message}"); }

            PinGodProcGame.MachineResourcesReady();
        }
        else { Logger.WarningRich(nameof(PinGodGameProc), "[color=yellow] no ProcScene found."); }
    }
    /// <summary>
    /// Adds machine items from <see cref="PinGodProcGame"/> into this Machine node <para/>
    /// TODO: perhaps this machine class overrides the base and uses the <see cref="PinGodProcGame"/> instead
    /// </summary>
    private void SetupPinGodotFromProcGame()
    {
        MachinePROC.ClearMachineItems();

        if (PinGodProcGame != null)
        {
            //add switches from p-roc
            if (PinGodProcGame.Switches?.Count > 0)
                PinGodProcGame.Switches.Values.ForEach(x => MachinePROC.AddSwitch(x.Name, (byte)x.Number));
            if (PinGodProcGame.Coils?.Count > 0)
                PinGodProcGame.Coils.Values.ForEach(x => MachinePROC.AddCoil(x.Name, (byte)x.Number));
            if (PinGodProcGame.Lamps?.Count > 0)
                PinGodProcGame.Lamps.Values.ForEach(x => MachinePROC.AddLamp(x.Name, (byte)x.Number));
            if (PinGodProcGame.LEDS?.Count > 0)
                PinGodProcGame.LEDS.Values.ForEach(x => MachinePROC.AddLed(x.Name, (byte)x.Number));
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

            //run proc game loop delay 1 save CPU //TODO: maybe this run loop needs to re-throw exception if any caught
            PinGodProcGame.RunLoop(PROC_DELAY, tokenSource);

            Logger.Info(nameof(MachinePROC), ":ending proc game loop");
            PinGodProcGame.EndRunLoop();
            Logger.Info(nameof(MachinePROC), ":proc game loop stopped");

            //this?.GetTree()?.Quit(0);

        }, tokenSource.Token);
    }

    /// <summary>
    /// Set window from database settings
    /// </summary>
    private void WindowLoadSettings()
    {
        DisplayServer.WindowSetMode((DisplayServer.WindowMode)Database.GetAdjustmentValue("DISP_MODE"));
        Display.SetContentScale(this, (Window.ContentScaleModeEnum)Database.GetAdjustmentValue("DISP_CONT_SCALE_MODE"));
        Display.SetAspectOption(this, (Window.ContentScaleAspectEnum)Database.GetAdjustmentValue("DISP_CONT_SCALE_ASPECT"));
        //get display size + pos from database values
        Display.SetSize(Database.GetAdjustmentValue("DISP_W"), Database.GetAdjustmentValue("DISP_H"));
        Display.SetPosition(Database.GetAdjustmentValue("DISP_X"), Database.GetAdjustmentValue("DISP_Y"));
        Display.SetAlwaysOnTop(Database.GetAdjustmentValue("DISP_TOP") > 0 ? true : false);
    }

    /// <summary>
    /// Save window adjustments to database from the main window
    /// </summary>
    private void WindowSaveSettings()
    {
        var winSize = DisplayServer.WindowGetSize(0);
        var winPos = DisplayServer.WindowGetPosition(0);
        Database.SetAdjustmentValue("DISP_W", winSize.x); Database.SetAdjustmentValue("DISP_H", winSize.y);
        Database.SetAdjustmentValue("DISP_X", winPos.x); Database.SetAdjustmentValue("DISP_Y", winPos.y);
        Database.SetAdjustmentValue("DISP_TOP", DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.AlwaysOnTop) ? 1 : 0);
        Database.SetAdjustmentValue("DISP_MODE", (int)DisplayServer.WindowGetMode());
        Database.SetAdjustmentValue("DISP_CONT_SCALE_MODE", (int)Display.GetContentScale(this));
        Database.SetAdjustmentValue("DISP_CONT_SCALE_ASPECT", (int)Display.GetAspectOption(this));
    }
}
