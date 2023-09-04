using Godot;
using System;

public partial class MainMenuGridContainer : ButtonGridGontainer
{    
	[Signal] public delegate void MenuItemSelectedEventHandler(string name);

	public override void _Ready()
	{
		SelectFirstChild();
	}

	private void _on_pg_menu_button_gui_input(InputEvent @event)
	{
		SendUI(@event, "tests");
	}

	private void SendUI(InputEvent @event, string name)
	{
		if (@event.IsActionPressed("ui_accept"))
		{
			// Replace with function body.
			EmitSignal(nameof(MenuItemSelected), name);
		}
	}
}
