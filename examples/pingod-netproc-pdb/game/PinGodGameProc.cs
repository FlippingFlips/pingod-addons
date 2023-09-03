using NetProc.Domain;
using PinGod.Core;
using PinGod.Game;
using System.Threading.Tasks;
using System.Threading;
using Godot;
using PinGod.Core.Service;
using NetProc.Domain.PinProc;
using PinGod.Core.Game;
using Microsoft.EntityFrameworkCore.Internal;
using System;

/// <summary>
/// Inheriting PinGodGame to take over with a P-ROC.
/// </summary>
public partial class PinGodGameProc : PinGodGame
{              
	[ExportCategory("P-ROC Options")]
	/// <summary>
	/// This delay set to 10 for the simulator. Trough won't work 100% without this. Hacks...
	/// </summary>
	[Export] byte PROC_DELAY = 10;
	[Export] bool DELETE_DB_ON_INIT = false;
	[Export] bool SIMULATED = true;
	[Export] bool IGNORE_DB_DISPLAY_SETTINGS = false;
	[Export(PropertyHint.Enum)] LogLevel LOG_LEVEL =  NetProc.Domain.PinProc.LogLevel.Verbose;

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

	#region Godot Overrides

	public override void _EnterTree()
	{
		Logger.LogLevel = (PinGod.Base.LogLevel)(LOG_LEVEL);
		base._EnterTree();
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
			CreateProcGame();
			Logger.Info(nameof(PinGodGameProc), ": ProcGame created. Setting up MachineNode from ProcGame.");

			//PinGodProcGame.Logger. = NetProc.Domain.PinProc.LogLevel.Verbose;
			var lvl = (int)LOG_LEVEL;
			Logger.LogLevel = (PinGod.Base.LogLevel)lvl;

			//SET MACHINE ITEMS FROM PROC TO PINGOD            
			//SetupPinGodotFromProcGame();
			Logger.Info(nameof(MachinePROC), ": ProcGame and database setup complete.");

			WindowLoadSettings();
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
	/// Must set solution / project to x86 if running real p-roc board
	/// true here is simulated and can get away with running with FakePinProc            
	/// </summary>
	/// <param name="machineConfig"></param>
	private void CreateProcGame()
	{
		var pinGodLogger = new PinGodProcLogger() { LogLevel = LOG_LEVEL };
		PinGodProcGame = new PinGodProcGameController(MachineType.PDB, DELETE_DB_ON_INIT, pinGodLogger, SIMULATED, this);

		//don't need to use LoadConfig anymore with this controller
		//PinGodProcGame.LoadConfig(machineConfig);
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
			PinGodProcGame.RunLoop(PROC_DELAY, tokenSource);
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
		if (IGNORE_DB_DISPLAY_SETTINGS) return;

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
		if (IGNORE_DB_DISPLAY_SETTINGS) return;

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
		}
	}
}
