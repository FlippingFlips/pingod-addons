using Godot;
using PinGod.Core;
using PinGod.Core.Service;
using PinGod.Game;
using System;
using System.Linq;
using static MoonStation.GameGlobals;

public enum ShipState
{
	None,
	Left,
	Right
}

public partial class MoonLander : Node2D
{
	[Signal] public delegate byte FlagEnteredEventHandler(byte value);
	[Signal] public delegate byte ModeOverEventHandler(int value);

	private static Random _random = new Random();
	private PackedScene _flagInstance;
	private ShipState _shipState;
	bool completed = false;
	Flag[] Flags = new Flag[4];
	private Label scoreLabel;
	private RigidBody2D ship;
	private MachineNode _machine;

	public override void _EnterTree()
	{
		base._EnterTree();

		ship = GetNode("PlayerShip") as RigidBody2D;
		_flagInstance = GD.Load("moonstation_lander/flag.tscn") as PackedScene;
		scoreLabel = GetNode("Label") as Label;
		scoreLabel.Visible = false;
	}

	public override void _ExitTree()
	{
		if (_machine != null)
		{
			_machine.SwitchCommand -= OnSwitchCommandHandler;
			_machine = null;
		}
		base._ExitTree();

	}

/*
	//testing with ui_left = Left, Right arrows
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed("ui_left"))
		{
			OnSwitchCommandHandler("flipperLwL", 0, 1);
		}
		else if (@event.IsActionPressed("ui_right"))
		{
			OnSwitchCommandHandler("flipperLwR", 0, 1);
		}
	}
*/

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (!completed)
		{
			if (_shipState == ShipState.Left)
			{
				var force = ship.ConstantForce.X <= -36f;
				if (!force)
					ship.ApplyCentralForce(new Vector2(-144, 0));
			}
			else if (_shipState == ShipState.Right)
			{
				var force = ship.ConstantForce.X >= 36f;
				if (!force)
					ship.ApplyCentralForce(new Vector2(144, 0));
			}
		}
		else
		{
			SetPhysicsProcess(false);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (HasNode("/root/Machine"))
		{
			_machine = GetNode<MachineNode>("/root/Machine");
			_machine.SwitchCommand += OnSwitchCommandHandler;
		}

		SetUpFlags();
	}

	public void OnMoonBody(Node2D body)
	{
		Logger.Debug(nameof(MoonLander), ": hit the moon");
		MoonLanderComplete(1);
	}

	private void MoonLanderComplete(byte value)
	{
		if (!completed)
		{
			completed = true;
			scoreLabel.Text = $"{Tr("CRATER_LAND")}{value * 1000}";
			scoreLabel.Visible = true;

			Logger.Debug(nameof(MoonLander), ": flag vaue=", value);
			ship.QueueFree();
			ship = null;
			foreach (var flag in Flags)
			{
				flag.QueueFree();
			}

			this.SetProcessInput(false);

			//signal video mode over
			EmitSignal(nameof(ModeOver), value * EXTRA_LARGE_SCORE);
		}
	}

	byte OnFlagEntered(byte value)
	{
		MoonLanderComplete(value);
		return value;
	}

	private void OnSwitchCommandHandler(string name, byte index, byte value)
	{
		if (!ship?.Visible ?? false) return;
		switch (name)
		{
			case "flipperLwL": //TODO:
				if (value <= 0)
					_shipState = ShipState.None;
				else
					_shipState = ShipState.Left;
				break;
			case "flipperLwR":
				if (value <= 0)
					_shipState = ShipState.None;
				else
					_shipState = ShipState.Right;
				break;
			default:
				break;
		}
	}

	private void SetUpFlags()
	{
		var array = Enumerable.Range(_random.Next(4), 4).ToArray();

		var flag = _flagInstance.Instantiate() as Flag;
		flag.RotationDegrees = -35.5f; flag.Position = new Vector2(36.5f, 301.1f);
		flag.SetMultiplier((byte)array[0]);
		Flags[0] = flag;


		flag = _flagInstance.Instantiate() as Flag;
		flag.RotationDegrees = -17.6f; flag.Position = new Vector2(133.5f, 249.4f);
		flag.SetMultiplier((byte)array[1]);
		Flags[1] = flag;

		flag = _flagInstance.Instantiate() as Flag;
		flag.RotationDegrees = 16.8f; flag.Position = new Vector2(357.81f, 241.9f);
		flag.SetMultiplier((byte)array[2]);
		Flags[2] = flag;

		flag = _flagInstance.Instantiate() as Flag;
		flag.RotationDegrees = 30.4f; flag.Position = new Vector2(467.43f, 296.08f);
		flag.SetMultiplier((byte)array[3]);
		Flags[3] = flag;

		for (int i = 0; i < Flags.Length; i++)
		{
			AddChild(Flags[i]);
		}

		this.FlagEntered += OnFlagEntered;
	}
}
