using Godot;
using NetProc.Data;
using PinGodNetProcPDB.scenes.ServiceMode.Shared;
using System.Linq;

public partial class ServiceModePinGod : Node
{	
	const string MAIN_CONTENT_NODE = "%ServiceModeCenterContainer";

	private PinGodGameProc _pinGodProcGame;
	private CenterContainer _mainContentNode;

	[Export(PropertyHint.File)] public Godot.Collections.Dictionary<string, string> _menuScenes;

	private string _previousMenu = "MainMenu";
	private string _currentMenu = "MainMenu";

	public static System.Collections.Generic.Dictionary<string, ServiceColourSet> ServiceModeColours { get; set; }

	#region Godot Overrides

	public override void _EnterTree()
	{
		base._EnterTree();

		//get the pingodgame to interact with
		_pinGodProcGame = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");

		//create color dictionary
		GetDatabaseColorSets();

		//get the main content to show views in
		_mainContentNode = GetNode<CenterContainer>(MAIN_CONTENT_NODE);
	}

	public override void _Ready()
	{
		LoadMenu(_currentMenu);
	}

	#endregion


	/// <summary>
	/// Populates the <see cref="ServiceModeColours"/> from database sets.
	/// </summary>
	private void GetDatabaseColorSets()
	{
		var serviceMappedColours = NetProcDataGameController.Database.Colors.Select(x => new ServiceColourSet
		{
			Name = x.Name,
			Color = Color.FromHtml(x.HtmlCode)
		});

		ServiceModeColours = serviceMappedColours.ToDictionary(x => x.Name);
	}

	private void _on_pg_menu_button_pressed()
	{
		// Replace with function body.
		GD.Print("service button menu enter");
	}

	private void OnMenuItemSelected(string name)
	{
		GD.Print("menu item selected: " + name);

		if(!_menuScenes.ContainsKey(name))
		{
			GD.PushError("no scene found for: " + name);
		}
		else
		{
			LoadMenu(name);			
		}	
	}

	/// <summary>
	/// Switch pressed from the PROC
	/// </summary>
	/// <param name="swName"></param>
	public void OnSwitchPressed(string swName, ushort swNum, bool isClosed)
	{
		if(!this.IsQueuedForDeletion())
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
				if (_currentMenu == "MainMenu")
				{
					_pinGodProcGame.RemoveMode("service");
					_pinGodProcGame.AddMode("attract");
					this.QueueFree();
				}
				else
				{
					LoadMenu(_previousMenu);
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

	/// <summary>
	/// Removes the current loaded scene from the MainContentNode and adds a scene from the <see cref="_menuScenes"/> dictionary
	/// </summary>
	/// <param name="sceneName"></param>
	private void LoadMenu(string sceneName)
	{
		//remove previous menu
		var child = _mainContentNode.GetChild(0);
		if(child != null)
		{
			_mainContentNode.RemoveChild(child);
			child.QueueFree();
		}

		//add scene
		var scene = ResourceLoader.Load(_menuScenes[sceneName]) as PackedScene;
		var menuGridContainer = scene.Instantiate();
		_mainContentNode.AddChild(menuGridContainer);

		GetNode<Label>("%TitleLabel").Text = $"Pingod Service Menu - " + sceneName;

		if (menuGridContainer as ButtonGridGontainer != null)
		{
			menuGridContainer.Connect("MenuItemSelected", Callable.From<string>(OnMenuItemSelected));
			
			_previousMenu = _currentMenu;
			_currentMenu = sceneName;
		}
	}
}
