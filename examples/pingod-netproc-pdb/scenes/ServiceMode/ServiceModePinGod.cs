using Godot;
using System.Linq;

public partial class ServiceModePinGod : Node
{
	private CenterContainer _centerContainer;
	private ButtonGridGontainer _mainMenugridContainer;
	private PackedScene _pgServiceButtonScene;

	private PinGodGameProc _pinGodProcGame;
	private ButtonGridGontainer _testsMenugridContainer;
	private Control _SwitchMatrixView;

	public override void _EnterTree()
	{
		base._EnterTree();
		_pinGodProcGame = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");
	}

	public override void _Ready()
	{
		_centerContainer = GetNodeOrNull<CenterContainer>("%ServiceModeCenterContainer");
		_mainMenugridContainer = _centerContainer.GetNodeOrNull<ButtonGridGontainer>("VBoxContainer/MainMenuGridContainer");
		_mainMenugridContainer.FocusMode = Control.FocusModeEnum.All;

		_testsMenugridContainer = _centerContainer.GetNodeOrNull<ButtonGridGontainer>("VBoxContainer/TestsMenuGridContainer");
		_SwitchMatrixView = _centerContainer.GetNodeOrNull<MarginContainer>("VBoxContainer/MarginContainer");
	}

	private void _on_pg_menu_button_pressed()
	{
		// Replace with function body.
		GD.Print("service button menu enter");
	}

	private void OnMenuItemSelected(string name)
	{
		GD.Print("menu item selected " + name);
		
		if (!string.IsNullOrWhiteSpace(name))
		{
			if(name == "tests")
			{
				_mainMenugridContainer.Visible = false;
				_testsMenugridContainer.Visible = true;
				_testsMenugridContainer.SelectFirstChild();
			}
			else if(name == "switches")
			{
				_mainMenugridContainer.Visible = false;
				_testsMenugridContainer.Visible = false;
				_SwitchMatrixView.Visible=true;
			}
			
			if(_mainMenugridContainer.Visible) _mainMenugridContainer.GrabClickFocus();
		}			
	}

	/// <summary>
	/// Switch pressed from the PROC
	/// </summary>
	/// <param name="swName"></param>
	public void OnSwitchPressed(string swName, ushort swNum, bool isClosed)
	{
		GetTree().CallGroup("switch_views", "OnSwitch", swName, swNum, isClosed);        
	}

	/// <summary>
	/// Switches sent from the P-ROC. This controls the UI input events
	/// </summary>
	/// <param name="swName"></param>
	public void OnServiceButtonPressed(string swName)
	{
		

		//_gridContainer.CallDeferred("grab_focus");

		var evt = new InputEventAction() { Action = "ui_right", Pressed = true };
		switch (swName)
		{
			case "exit":
				if (_mainMenugridContainer.Visible)
				{
					_pinGodProcGame.RemoveMode("service");
					_pinGodProcGame.AddMode("attract");
					this.QueueFree();
				}
				else
				{
					_SwitchMatrixView.Visible = false;
					_testsMenugridContainer.Visible = false;
					_mainMenugridContainer.Visible = true;
					_mainMenugridContainer.SelectFirstChild();
				}
				break;
			case "enter":
				evt.Action = "ui_accept";
				_pinGodProcGame.PlaySfx("credit");
				break;
			case "down":
				evt.Action = "ui_left";
				break;
			case "up":
			default:
				break;
		}
		
		Input.ParseInputEvent(evt);
	}
}


