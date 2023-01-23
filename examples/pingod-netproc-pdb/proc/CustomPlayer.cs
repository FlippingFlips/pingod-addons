using NetProc.Domain.Players;

public class CustomPlayer : Player
{
    public CustomPlayer(string name) : base(name)
    {
    }

    public bool[] TargetsCompleted { get; set; } = new bool[4];
}
