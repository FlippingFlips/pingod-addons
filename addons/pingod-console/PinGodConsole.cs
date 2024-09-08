using Godot;
using PinGod.EditorPlugins;

/// <summary>Used as an autoload singleton to hook onto events from the static Logger<para/>
/// It's just a way of using the std out. <para/>
/// TODO: add console commands if necessary</summary>
public partial class PinGodConsole : Node
{
    private RichTextLabel _label;

    [Export] PackedScene _consoleWindowScene;

    WindowPinGod Window;

    public override void _EnterTree()
    {
        base._EnterTree();

        Window = _consoleWindowScene?.InstantiateOrNull<WindowPinGod>();
        var root = this.GetTree().Root;
        root.CallDeferred("add_child", Window);
        Window?.Show();
    }

    public override void _Ready()
    {
        base._Ready();        
        Logger.LoggedMessage += Logger_LoggedMessage;
    }

    /// <summary>Console window with the ` quote left key</summary>
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Window != null && @event is InputEventKey k)
        {            
            if (k.KeyLabel == Key.Quoteleft && k.Pressed)
            {
                Window.Visible = !Window.Visible;
            }
        }

        base._UnhandledKeyInput(@event);
    }

    /// <summary>The text label to update whenever a log happens</summary>
    /// <param name="richTextLabel"></param>
    public void SetTextLabelToUpdate(RichTextLabel richTextLabel) => 
        _label = richTextLabel;

    /// <summary>Call back to push append messages onto a rich text label</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Logger_LoggedMessage(object sender, string e) => 
        _label?.CallDeferred("append_text", "\n" + e);
}
