using Godot;
using PinGod.EditorPlugins;

/// <summary>HACK: The refresh button will invoke `_on_button_button_down`<para/>
/// This reads the latest log and current log being used from the UserDataDir/logs/godot.log</summary>
public partial class ConsoleWindow : WindowPinGod
{
    private string _usrDir;
    private Label _label;
    private ScrollContainer _scroll;

    /// <summary></summary>
    public override void _Ready() 
	{
		_usrDir = System.IO.Path.Combine(OS.GetUserDataDir(), "logs", "godot.log");
        _label = this.GetNode<Label>("ScrollContainer/Label");
        _scroll = this.GetNode<ScrollContainer>("ScrollContainer");

        //refresh log
        _on_button_button_down();
    }

    /// <summary>figure out a better way to redirect stdout or similar, through the logger</summary>
    /// <returns></returns>
	string GetLogContents()
	{
        using var file = FileAccess.Open(_usrDir, FileAccess.ModeFlags.Read);
        return file.GetAsText();
    }

    /// <summary>Read the log file contents, set the label and move the scrollbar</summary>
    protected void _on_button_button_down() 
    {
        _label.Text = GetLogContents();
        _scroll.SetDeferred("scroll_vertical", 20000);
    }
}
