using NetProc.Data.Model;
using System.Collections.Generic;

public class PinGodPROCPlayer : NetProc.Domain.Players.Player
{
    public PinGodPROCPlayer(string name, int id = 0) : base(name)
    {
        Id = id;
        BallStats = new List<BallPlayed>();
    }

    /// <summary>
    /// database id
    /// </summary>
    public int Id { get; }
    public List<BallPlayed> BallStats { get; set; }
}
