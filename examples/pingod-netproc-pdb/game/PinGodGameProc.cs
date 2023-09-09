using NetProc.Domain;
using PinGod.Core;
using PinGod.Game;
using System.Threading.Tasks;
using System.Threading;
using Godot;
using PinGod.Core.Service;
using NetProc.Domain.PinProc;
using System;

public class PinGodProcConfig
{
	public bool Simulated { get; set; } = true;
	public byte Delay { get; set; } = 10;
	public bool IgnoreDbDisplay { get; set; }
	public bool DeleteDbOnInit { get; set; }
	public LogLevel LogLevel { get; set; } = LogLevel.Verbose;
}

/// <summary>
/// Inheriting PinGodGame to take over with a P-ROC.
/// </summary>
public partial class PinGodGameProc : PinGodGame
{              
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
	/// To cancel the PROC loop
	/// </summary>
	private CancellationTokenSource tokenSource;

	public CanvasLayer _modesCanvas;
	private Label _creditsLabel;

	public PinGodProcConfig PinGodProcConfig { get; private set; } = new();
    public bool GameReady { get; private set; }

    #region Godot Overrides

    public override void _EnterTree()
	{
		LoadLocalProcConfig();

		Logger.LogLevel = (PinGod.Base.LogLevel)((int)PinGodProcConfig.LogLevel);

		base._EnterTree();
	}

	/// <summary>
	/// Create a P-ROC config file for config that cannot be in the database. <para/>
	/// This way we can easy make changes to it for simulated and delays in the directory
	/// </summary>
	private void LoadLocalProcConfig()
	{
		var config = new ConfigFile();
		Error err = config.Load("res://proc.cfg");
		if (err != Error.Ok)
		{
			//create config
			config.SetValue("DEV", "simulated", PinGodProcConfig.Simulated);
			config.SetValue("DEV", "delete_db_on_init", PinGodProcConfig.DeleteDbOnInit);
			config.SetValue("DEV", "ignore_db_display", PinGodProcConfig.IgnoreDbDisplay);
			config.SetValue("PROC", "delay", PinGodProcConfig.Delay);
			config.SetValue("DEV", "log_level", (int)PinGodProcConfig.LogLevel);

			config.Save("res://proc.cfg");
		}
		else
		{
			PinGodProcConfig.Delay = (byte)config.GetValue("PROC", "delay");
			PinGodProcConfig.IgnoreDbDisplay = (bool)config.GetValue("DEV", "ignore_db_display");
			PinGodProcConfig.DeleteDbOnInit = (bool)config.GetValue("DEV", "delete_db_on_init");
			PinGodProcConfig.Simulated = (bool)config.GetValue("DEV", "simulated");
			PinGodProcConfig.LogLevel = (NetProc.Domain.PinProc.LogLevel)((int)config.GetValue("DEV", "log_level"));
		}

		GD.Print("log level: " + PinGodProcConfig.LogLevel);
	}

	/// <summary>
	/// Quit the P-ROC game loop
	/// </summary>
	public override void _ExitTree()
	{
		//set display size + pos to database
		WindowSaveSettings();

		PinGodProcGame?.ExitGame();

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

		_modesCanvas = GetNodeOrNull<CanvasLayer>("/root/ProcScene/Modes");

		MachinePROC = this.MachineNode as MachinePROC;
		if (MachinePROC != null)
		{
			//CREATE AND SETUP PROC MACHINE
			try
			{
				CreateProcGame();

				Logger.Info(nameof(PinGodGameProc), ": ProcGame created. Setting up MachineNode from ProcGame.");

				//PinGodProcGame.Logger. = NetProc.Domain.PinProc.LogLevel.Verbose;
				var lvl = (int)LogLevel;
				Logger.LogLevel = (PinGod.Base.LogLevel)lvl;

				if (PinGodProcGame == null)
				{
					throw new NullReferenceException("P-ROC game couldn't be created. Simulated: " + PinGodProcConfig.Simulated);
				}					

				//SET MACHINE ITEMS FROM PROC TO PINGOD            
				//SetupPinGodotFromProcGame();
				Logger.Info(nameof(MachinePROC), ": ProcGame and database setup complete.");

				WindowLoadSettings();
			}
			catch (System.DllNotFoundException dllEx)
			{
				LogError(dllEx.Message + ". Application should be built with 32bit to use PinProc libraries when not simualted.");
				this.GetTree().Quit();
				
			}
			catch (System.Exception ex)
			{
				LogError(ex.Message);
				this.GetTree().Quit();
			}            
		}
	}
	#endregion

	public override void AddCredits(byte amt)
	{        
		Credits += amt;

		//let any other scenes know we've updated the credits
		EmitSignal(nameof(CreditAdded), Credits);
	}

	/// <summary> Adds a mode to the PROC modes </summary>
	/// <param name="mode"></param>
	public void AddMode(PinGodProcMode mode) => PinGodProcGame.Modes.Add(mode);

	public void AddMode(string mode) => PinGodProcGame.AddMode(mode);

	/// <summary> Removes a mode from the PROC modes </summary>
	/// <param name="mode"></param>
	public void RemoveMode(PinGodProcMode mode) => PinGodProcGame.Modes.Remove(mode);

	public void RemoveMode(string mode) => PinGodProcGame.RemoveMode(mode);

	public void RemoveAttractMode() => PinGodProcGame.Modes.Remove(PinGodProcGame._AttractMode);

	/// <summary>
	/// Override the default window setup
	/// </summary>
	public override void SetupWindow() 
	{
		//base.SetupWindow();        
	}

	/// <summary>
	/// Must set solution / project to x86 if running real p-roc board. <para/>
	/// Use the <see cref="PinGodProcConfig.Simulated"/> flag from the PROC.cfg in game directory
	/// </summary>
	/// <param name="machineConfig"></param>
	private void CreateProcGame()
	{
		var pinGodLogger = new PinGodProcLogger() { LogLevel = PinGodProcConfig.LogLevel };
		PinGodProcGame = new PinGodProcGameController(MachineType.PDB, PinGodProcConfig.DeleteDbOnInit, pinGodLogger, PinGodProcConfig.Simulated, this);        
	}

	/// <summary>
	/// Runs the P-ROC game loop  <para/>
	/// The Resources node has loaded all resources. Now we can get any packed scenes and use in modes <para/>
	/// </summary>
	private void OnResourcesLoaded()
	{
		var ms = GetNodeOrNull<Node>("/root/ProcScene");
		if (ms != null)
		{
			try
			{
				StartProcGameLoop();

				PinGodProcGame.MachineResourcesReady();

				GameReady = true;
			}
			catch (System.Exception ex) { Logger.Error(nameof(MachinePROC), nameof(_Ready), $"{ex.Message} - {ex.InnerException?.Message}"); }			
		}
		else { Logger.WarningRich(nameof(PinGodGameProc), "[color=yellow] no ProcScene found.[/color]"); }
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
			PinGodProcGame.RunLoop(PinGodProcConfig.Delay, tokenSource);
			//means the proc loop threw exception
			if (!tokenSource.IsCancellationRequested)
			{
				this?.GetTree()?.Quit(0);
			}
			else
			{
				Logger.Info(nameof(MachinePROC), ":ending proc game loop");
				PinGodProcGame.EndRunLoop();
				Logger.Info(nameof(MachinePROC), ":proc game loop stopped");
			}            

		}, tokenSource.Token);
	}

	public void StartProcGame()
	{
		PinGodProcGame.Logger.Log("start button, trough full");
		PinGodProcGame.StartGame();
		PinGodProcGame.AddPlayer();
		PinGodProcGame.StartBall();
		PinGodProcGame.IncrementAudit("CREDITS_TOTAL", 1);
		PinGodProcGame.IncrementAudit("CREDITS", -1);
		Credits--;

		PinGodProcGame.RemoveMode("attract");
	}

	/// <summary>
	/// Set window from database settings. Return if <see cref="IGNORE_DB_DISPLAY_SETTINGS"/> is set to true
	/// </summary>
	private void WindowLoadSettings()
	{
		if (PinGodProcGame == null) return;
		
		if (PinGodProcConfig.IgnoreDbDisplay) return;        

		DisplayServer.WindowSetMode((DisplayServer.WindowMode)PinGodProcGame.GetAdjustment("DISP_MODE"));
		Display.SetContentScale(GetTree().Root, (Window.ContentScaleModeEnum)PinGodProcGame.GetAdjustment("DISP_CONT_SCALE_MODE"));
		Display.SetAspectOption(GetTree().Root, (Window.ContentScaleAspectEnum)PinGodProcGame.GetAdjustment("DISP_CONT_SCALE_ASPECT"));
		//get display size + pos from database values
		Display.SetSize(PinGodProcGame.GetAdjustment("DISP_W"), PinGodProcGame.GetAdjustment("DISP_H"));
		Display.SetPosition(PinGodProcGame.GetAdjustment("DISP_X"), PinGodProcGame.GetAdjustment("DISP_Y"));
		Display.SetAlwaysOnTop(PinGodProcGame.GetAdjustment("DISP_TOP") > 0 ? true : false);
	}

	/// <summary>
	/// Save window adjustments to database from the main window. Return if <see cref="IGNORE_DB_DISPLAY_SETTINGS"/> is set to true
	/// </summary>
	private void WindowSaveSettings()
	{
		if (PinGodProcGame == null) return;
		
		if (PinGodProcConfig.IgnoreDbDisplay) return;		

		var winSize = DisplayServer.WindowGetSize(0);
		var winPos = DisplayServer.WindowGetPosition(0);
		PinGodProcGame.SetAdjustment("DISP_W", winSize.X); PinGodProcGame.SetAdjustment("DISP_H", winSize.Y);
		PinGodProcGame.SetAdjustment("DISP_X", winPos.X); PinGodProcGame.SetAdjustment("DISP_Y", winPos.Y);
		PinGodProcGame.SetAdjustment("DISP_TOP", DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.AlwaysOnTop) ? 1 : 0);
		PinGodProcGame.SetAdjustment("DISP_MODE", (int)DisplayServer.WindowGetMode());
		PinGodProcGame.SetAdjustment("DISP_CONT_SCALE_MODE", (int)Display.GetContentScale(GetTree().Root));
		PinGodProcGame.SetAdjustment("DISP_CONT_SCALE_ASPECT", (int)Display.GetAspectOption(GetTree().Root));
	}

	public Node AddModeScene(string scenePath)
	{
		if (_resources != null)
		{
			//get the pre loaded resource, create instance and add to base mode canvas
			var scene = _resources?.GetResource(scenePath.GetBaseName()) as PackedScene;
			var instance = scene.Instantiate();

			PinGodProcGame.Logger.Log($"instance null: " + instance == null);

			_modesCanvas.AddChild(instance);

			return instance;
		}
		else
		{
			Logger.WarningRich(nameof(PinGodGameProc), nameof(AddModeScene), ": [color=yellow]no resources found, can't create attract scene[/color]"); 
			return null;
		}
	}

	/// <summary>
	/// Remove mode from the modes canvas
	/// </summary>
	/// <param name="sceneName"></param>
	internal void RemoveModeScene(string sceneName)
	{
		var node = _modesCanvas.GetNodeOrNull(sceneName);
		if(node != null)
		{
			_modesCanvas.RemoveChild(node);
			node.QueueFree();
		}
	}
}
