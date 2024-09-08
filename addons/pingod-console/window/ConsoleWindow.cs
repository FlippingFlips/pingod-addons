using Godot;
using PinGod.EditorPlugins;
using System.Collections.Generic;

/// <summary>Passes the text label onto the PinGodConsole<para/>
/// The PGConsole is hooked up to logger events and prints to the text label</summary>
public partial class ConsoleWindow : WindowPinGod
{
    private string _usrDir;
    private RichTextLabel _label;
    private LineEdit _lineEdit;
    Stack<string> _commands = new();

    /// <summary></summary>
    public override void _EnterTree() 
	{
        //_usrDir = System.IO.Path.Combine(OS.GetUserDataDir(), "logs", "godot.log");

        _label = this.GetNode<RichTextLabel>("VBoxContainer/Control/Label");
        _lineEdit = this.GetNode<LineEdit>("VBoxContainer/LineEdit");

        var pgConsole = GetNodeOrNull<PinGodConsole>(Paths.ROOT_CONSOLE);
        if (pgConsole == null)
            Logger.Error(nameof(ConsoleWindow), ":PinGodConsole not found");
        else
            pgConsole.SetTextLabelToUpdate(_label);

        _lineEdit.TextSubmitted += _lineEdit_TextSubmitted;
    }

    public override void PinGodWindow_CloseRequested() => this.Hide();

    private void _lineEdit_TextSubmitted(string newText)
    {
        _lineEdit.Clear();

        //add the command to get again, keep 10
        _commands.Push(newText);
        if (_commands.Count > 10) { _commands.Pop(); }        

        //test on a help command
        if(newText != null && newText == "/help") { Logger.Info("/help sent"); }
    }

    /// <summary>Console window with the ` quote left key</summary>
    /// <param name="event"></param>
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey k)
        {
            if (k.KeyLabel == Key.Quoteleft && k.Pressed)
            {
               Visible = !Visible;
            }
        }

        base._UnhandledKeyInput(@event);
    }
}
