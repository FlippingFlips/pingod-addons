using PinGod.Game;

/// <summary>When this class script is set in the `autoload\PinGodGame.tscn` it replaces the autoload singleton IPinGodGame.</summary>
public partial class DemoPinGodGame : PinGodGame
{
    /// <summary>Override when the player is created to add our own player.</summary>
    /// <param name="name"></param>
    public override void CreatePlayer(string name)
    {
        //don't call the base
        //base.CreatePlayer(name);

        //add a new player
        Players.Add(new DemoPlayer
        {
            Name = name,
            Points = 0,
            Initials = "AAA"
        });
    }
}
