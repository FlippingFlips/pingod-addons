using Godot;

/// <summary>
/// Custom class of <see cref="PinGodGame"/> to override when a player is added
/// </summary>
public partial class CustomPinGodGame : PinGodGame
{
    /// <summary>
    /// override to create our own player type for this game
    /// </summary>
    /// <param name="name"></param>
    public override void CreatePlayer(string name)
    {
        Players.Add(new BasicGamePlayer() { Name = name, Points = 0 });
    }

    public override void _Ready()
    {
        base._Ready();

        if (HasNode(nameof(Resources)))
        {
            LogInfo(nameof(CustomPinGodGame), ":resources pre loader node found");
        }
        if (HasNode(nameof(Trough)))
        {
            LogInfo(nameof(CustomPinGodGame), ":Trough node found");
        }
        if (HasNode(nameof(AudioManager)))
        {
            LogInfo(nameof(CustomPinGodGame), ":AudioManager node found");
        }
    }

    /// <summary>
    /// Logs when this class is setup, nothing more.
    /// </summary>
    public override void Setup()
    {
        base.Setup();
        LogInfo(nameof(CustomPinGodGame), ":setup custom game finished");

        //get the root viewport
        GetTree().Root.SizeChanged += on_size_changed;


    }

    private void on_size_changed()
    {
        Logger.Verbose(nameof(CustomPinGodGame), ":size changed");
    }
}