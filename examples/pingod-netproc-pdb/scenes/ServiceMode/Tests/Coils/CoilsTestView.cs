using Godot;

/// <summary>
/// Simple coil items list in a GridContainer. <para/>
/// Should be able to up/down through the list and test coils with default pulse time
/// </summary>
public partial class CoilsTestView : Control
{
	private PinGodGameProc _pinGodProcGame;
	private GridContainer _coilsList;

	[Export(PropertyHint.File)] string GridContainerItemScene 
		= "res://scenes/ServiceMode/Tests/Coils/CoilGridContainerItem.tscn";
	private PackedScene _gridItem;
	private CoilTestItem[] _coilItems;
	
	private int _selectedCoilItemIndex = -1;

	public override void _EnterTree()
	{
		_gridItem = ResourceLoader.Load(GridContainerItemScene) as PackedScene;

		//item container for coils, selectable
		_coilsList = GetNode<GridContainer>("%CoilsItemList");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pinGodProcGame = GetNodeOrNull<PinGodGameProc>("/root/PinGodGame");

		_coilsList.GrabClickFocus();

		var configCoils = _pinGodProcGame.PinGodProcGame.Config.PRCoils;

		_coilItems = new CoilTestItem[configCoils.Count];
		int i = 0;

		//get the config coils, not from machine
		foreach (var coil in _pinGodProcGame.PinGodProcGame.Config.PRCoils)
		{
			var item = _gridItem.Instantiate() as Control;
			item.FocusMode = FocusModeEnum.All;		

			//set the items labels and colours for this coil
			item.GetNode<Label>("%CoilNameLabel").Text += $"{coil.Number}    {coil.Name}";
			item.GetNode<Label>("%GroundWireLabel").Text = coil.ReturnWire;
			item.GetNode<Label>("%VoltageWireLabel").Text = coil.VoltageWire;
			item.GetNode<Label>("%VoltageLabel").Text = coil.Voltage.HasValue ? 
				$"{coil.Voltage}v" : "48v";

			//set up displaying any wire colous set on switches
			if (!string.IsNullOrWhiteSpace(coil.ReturnWire) && !string.IsNullOrWhiteSpace(coil.VoltageWire))
			{
				//get the wire colour names from database config
				var voltageWireColours = coil.VoltageWire.Split(",");
				var returnWireColours = coil.ReturnWire.Split(",");

				var rColorRect = item.GetNode<ColorRect>("%RtnWireColorRect");
				var vColourRect = item.GetNode<ColorRect>("%VoltageWireColorRect");

				if (returnWireColours.Length > 0)
				{
					rColorRect.Visible = true;
					rColorRect.Color = 
						ServiceModePinGod.ServiceModeColours[returnWireColours[0]].Color;
				}
				if(returnWireColours.Length > 1)
				{
					var cRect = rColorRect.GetNode<ColorRect>("ColorRect2");
					cRect.Visible = true;
					cRect.Color =
						ServiceModePinGod.ServiceModeColours[returnWireColours[1]].Color;
				}

				if (voltageWireColours.Length > 0)
				{
					vColourRect.Visible = true;
					vColourRect.Color =
						ServiceModePinGod.ServiceModeColours[voltageWireColours[0]].Color;
				}
				if (voltageWireColours.Length > 1)
				{
					var cRect = vColourRect.GetNode<ColorRect>("ColorRect2");
					cRect.Visible = true;
					cRect.Color =
						ServiceModePinGod.ServiceModeColours[voltageWireColours[1]].Color;
				}
			}

			_coilsList.AddChild(item);
			_coilItems[i] = new CoilTestItem { Control = item, Coil = coil };
			i++;
		}

		SetSelectedItem();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsAction("ui_left"))
		{
			GD.Print("ui_left");

			SetSelectedItem(false);
		}
		else if (@event.IsAction("ui_right"))
		{
			GD.Print("ui_right");

			SetSelectedItem(true);
		}
		//pulse the coil when user pushes enter on the item
		else if (@event.IsAction("ui_accept"))
		{
			GD.Print("accept");
			var coilItem = _coilItems[_selectedCoilItemIndex];

			_pinGodProcGame.PinGodProcGame
				.Coils[coilItem.Coil.Name]
				.Pulse(coilItem.Coil.PulseTime);
		}
	}

	private void SetSelectedItem(bool next = true)
	{	
		//use to set the hide previous selected
		if(_selectedCoilItemIndex > -1)
		{
			_coilItems[_selectedCoilItemIndex].Control.GrabFocus();
			_coilItems[_selectedCoilItemIndex].Control
				.GetNode<ColorRect>("SelectedColorRect").Visible = false;
		}			

		//Next item index
		if (next)
		{
			_selectedCoilItemIndex++;
			if (_selectedCoilItemIndex > _coilItems.Length - 1)
			{
				_selectedCoilItemIndex = 0;
			}
		}
		else
		{
			_selectedCoilItemIndex--;
			if(_selectedCoilItemIndex < 0)
				_selectedCoilItemIndex = _coilItems.Length - 1;
		}

		//set the colour to be selected on the item
		_coilItems[_selectedCoilItemIndex].Control
			.GetNode<ColorRect>("SelectedColorRect").Visible = true;

		_coilItems[_selectedCoilItemIndex].Control.GrabFocus();
	}
}
