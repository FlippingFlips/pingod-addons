using Godot;
using System;

/// <summary>
/// This button sends a signal from the button event: _on_gui_input <para/>
/// On "ui_accept" it will EmitSignal for the item name selected
/// </summary>
public partial class PgMenuButton : Button
{
	[Signal] public delegate void MenuItemSelectedEventHandler(string name);

	private void _on_gui_input(InputEvent @event)
	{        
		if (@event.IsActionPressed("ui_accept"))
		{
			EmitSignal(nameof(MenuItemSelected), Text);
		}
	}
}



