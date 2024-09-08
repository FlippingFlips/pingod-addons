using Godot;
using PinGod.EditorPlugins;
using System.Collections.Generic;
using System.Linq;

public partial class ToolsPanelWindow : WindowPinGod
{
    /// <summary>buttons in the window to do stuff</summary>
    private IEnumerable<Button> _buttons;
    private Label _debugLabel;
    private IPinGodGame pinGod;

    [Godot.Signal] public delegate void ShowHideWindowEventHandler(string name, bool show);

    public override void _EnterTree()
    {
        base._EnterTree();
		var btnGroup = GetNodeOrNull<HBoxContainer>("HBoxContainer");
		if(btnGroup == null) { Logger.Warning(nameof(ToolsPanelWindow), " no button group found"); }

		_buttons = btnGroup
			.GetChildren()
			.Where(x => x.GetType() == typeof(Button)).Cast<Button>();

        _debugLabel = btnGroup.GetNode<Label>("Label");

        Logger.Verbose($"{nameof(ToolsPanelWindow)}:{_buttons.Count()} buttons found");

        if (HasNode(Paths.ROOT_PINGODGAME))
        {
            pinGod = GetNode<IPinGodGame>(Paths.ROOT_PINGODGAME);
        }
        Logger.Info(nameof(Bonus), ":_EnterTree");
    }

    public override void PinGodWindow_CloseRequested()
    {
        //base.PinGodWindow_CloseRequested();
        this.Hide();
    }

    /// <summary>Sends a ShowHideWindow signal with the name and state</summary>
    /// <param name="name"></param>
    /// <param name="toggledOn"></param>
    public virtual void Button_Toggled(string name, bool toggledOn)
    {
        Logger.Verbose(nameof(ToolsPanelWindow), $" {name}={toggledOn}");
        EmitSignal(nameof(ShowHideWindow), name, toggledOn);
    }

    public override void _ExitTree() => base._ExitTree();

    /// <summary>Sets up callbacks for buttons found in the container when a button is toggled<para/>
    /// Invokes <see cref="Button_Toggled(string, bool)"/><para/>
    /// This will send a ShowHideWindow signal picked up by the parent window</summary>
    public override void _Ready()
	{
		base._Ready();
        foreach (var button in _buttons)
            button.Toggled += (x => Button_Toggled(button.Name, x));
    }

    public override void _Process(double delta)
    {
        _debugLabel.Text = $"Save active: {pinGod?.BallSaveActive}";

        base._Process(delta);
    }
}

