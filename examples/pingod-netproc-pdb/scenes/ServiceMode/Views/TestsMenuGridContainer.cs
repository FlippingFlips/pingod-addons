using Godot;
using System;

public partial class ButtonGridGontainer : GridContainer
{
	public void SelectFirstChild()
	{
		foreach (var item in GetChildren())
		{
			if (item is Button)
			{
				((Button)item).GrabFocus();
				break;
			}
		}
	}
}

public partial class TestsMenuGridContainer : ButtonGridGontainer
{    
	[Signal] public delegate void MenuItemSelectedEventHandler(string name);

	private void _on_pg_menu_button_gui_input(InputEvent @event)
	{
		SendUI(@event, "switches");
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
