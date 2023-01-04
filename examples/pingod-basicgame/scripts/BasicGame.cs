
using Godot;
/// <summary>
/// BasicGame with Modes tree (game/Game.tscn). This does nothing itself here but is loaded from Game.tscn <para/>
/// this.GetTree().Root.Connect("gui_focus_changed", this, "gui_focus_changed"); 
/// </summary>
public partial class BasicGame : Game
{
    //private void gui_focus_changed(Node node)
    //{
    //    Logger.Info(nameof(BasicGame), ":gui focus changed:" + node?.ToString());
    //}

    //TODO: Test OS.MoveWindowToForeground();
    //OS.Alert
    //Godot.PCKPacker p = new Godot.PCKPacker();
    //p.PckStart("test.pck");
    //p.AddFile("res://")

    /*
     * Create window inside the game
     private Window _window;
            _window = new Godot.Window() { Size = new Vector2i(300, 200) };
        AddChild(_window);
        _window.Show();
            _window.Hide();
        _window.QueueFree();
     */

    public void OnBallSaveDisabled()
    {
        Logger.Log(PinGodLogLevel.Info, nameof(BasicGame), ":"+nameof(OnBallSaveDisabled));
    }

    public void OnBallSaveEnabled()
    {
        Logger.Log(PinGodLogLevel.Info, nameof(BasicGame), ":" + nameof(OnBallSaveEnabled));
    }
}