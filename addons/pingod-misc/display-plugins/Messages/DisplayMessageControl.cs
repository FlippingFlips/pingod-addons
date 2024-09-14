using Godot;

/// <summary>Quick scene to pop up a message</summary>
public partial class DisplayMessageControl : Control
{
	[Export] float DisplayForAndRemove = 3.0f;
    [Export] string TextToDisplay = "Warning\nMessage";

    private Label _label;

    public override void _EnterTree()
    {
        base._EnterTree();
        _label = GetNode("%Label") as Label;
        _label.Text = TextToDisplay;
    }

    /// <summary></summary>
    public override void _Ready()
	{
		var _timer = GetNode<Timer>("Timer");
        _timer.WaitTime = DisplayForAndRemove;
        _timer.Start();
        _timer.Timeout += () => QueueFree();
	}

    /// <summary>Use before adding to the tree</summary>
    /// <param name="txt"></param>
    public void SetText(string txt) => TextToDisplay = txt;

    public void SetTime(float time) => DisplayForAndRemove = time;
}
