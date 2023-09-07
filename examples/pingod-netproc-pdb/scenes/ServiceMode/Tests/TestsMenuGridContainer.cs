using Godot;
using System;

public partial class ButtonGridGontainer : GridContainer
{
	[Signal] public delegate void MenuItemSelectedEventHandler(string name);

	public override void _Ready()
	{
		base._Ready();

		foreach (var item in GetChildren())
		{
			var btn = item as PgMenuButton;
			if(btn != null)
			{
				btn.MenuItemSelected += OnButtonSelected;
			}
		}
	}

	public void OnButtonSelected(string name)
	{
		//shift event again to parent
		EmitSignal(nameof(MenuItemSelected), name);
	}

	//public override void _ExitTree()
	//{
	//    base._ExitTree();

	//    foreach (var item in GetChildren())
	//    {
	//        var btn = item as PgMenuButton;
	//        if (btn != null)
	//        {
	//            btn.MenuItemSelected -= OnButtonSelected;
	//        }
	//    }
	//}

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
	public override void _Ready()
	{
		base._Ready();

		SelectFirstChild();
	}
}
