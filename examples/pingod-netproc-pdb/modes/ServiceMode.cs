using Godot;
using NetProc.Domain;
using PinGod.Core;

/// <summary>
/// P-ROC Mode that runs the service mode scenes
/// </summary>
public class ServiceMode : PinGodProcMode
{
	private string[] _doorSwitches;
	private PinGodGameProc _pinGodProc;
	private PackedScene _serviceModeScene;
	private Node _serviceModeInstance;

	public ServiceMode(IGameController game, IPinGodGame pinGod, string name = nameof(ServiceMode), int priority = 80, string defaultScene = null, bool loadDefaultScene = true) :
		base(game, name, priority, pinGod, defaultScene, loadDefaultScene)
	{
		//get all switches tagged as 'door' and add a AddSwitchHandler to invoke HandleDoorSwitch
		_doorSwitches = Game.Config.GetNamesFromTag("door", MachineItemType.Switch);
		
		if (_doorSwitches?.Length > 0)
		{
			//handle each switch when closed
			for (int i = 0; i < _doorSwitches.Length; i++)
			{
				AddSwitchHandler(_doorSwitches[i], SwitchHandleType.closed, 0, new SwitchAcceptedHandler(HandleDoorSwitch));
			}                
		}
		else { Game.Logger.Log("WARN: no door switches found.", NetProc.Domain.PinProc.LogLevel.Warning); }

		//PingodGame p-roc, use to get hold of the machine so we can add credits.
		_pinGodProc = pinGod as PinGodGameProc;
	}

	public override void LoadDefaultSceneToCanvas(string scenePath)
	{
		if(_modesCanvas != null)
		{
			var res = GD.Load<PackedScene>(scenePath);
			var inst = res.Instantiate();
			CanvasLayer?.AddChild(inst);
			_modesCanvas.AddChild(CanvasLayer);
		}
	}

	public override void ModeStarted()
	{
		if (_resources != null)
		{
			//get the pre loaded resource, create instance and add to base mode canvas
			_serviceModeScene = _resources?.GetResource(defaultScene) as PackedScene;
			_serviceModeInstance = _serviceModeScene.Instantiate();
			AddChildSceneToCanvasLayer(_serviceModeInstance);
		}
		else { Logger.WarningRich(nameof(AttractMode), nameof(ModeStarted), ": [color=yellow]no resources found, can't create attract scene[/color]"); }
	}

	bool HandleDoorSwitch(NetProc.Domain.Switch sw)
	{
		switch (sw.Name)
		{
			case "down":
				Game.Logger.Log("todo: volume down", NetProc.Domain.PinProc.LogLevel.Info);
				break;
			case "exit":
			case "up":
			case "enter":
				break;
			default:
				break;
		}
		return SWITCH_CONTINUE;
	}
}
