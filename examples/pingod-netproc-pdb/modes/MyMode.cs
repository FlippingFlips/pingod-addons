using NetProc.Domain;
using NetProc.Domain.PinProc;
using PinGod.Game;
using Switch = NetProc.Domain.Switch;

public partial class MyMode : PinGodProcMode
{
    public MyMode(IGameController game, int priority, PinGodGame pinGod, string name = nameof(MyMode)) : base(game, name, priority, pinGod) 
    {
        //add switch handler for all bumpers
        for (int i = 0; i < 3; i++) { AddSwitchHandler($"bumper{i}", SwitchHandleType.active, 0, new SwitchAcceptedHandler(OnBumperHit)); }

        var targBankSwitches = Game.Config.GetNamesFromTag("targetBank", MachineItemType.Switch);
        foreach (var item in targBankSwitches) { AddSwitchHandler(item, SwitchHandleType.open, 0, new SwitchAcceptedHandler(OnTargetHit)); }

        //_player = game.CurrentPlayer();
    }

    /// <summary>
    /// Launches a ball from the trough
    /// </summary>
    public override void ModeStarted()
    {
        Game.Logger.Log(GetType().Name+":"+nameof(ModeStarted), LogLevel.Debug);
        _game.Trough.LaunchBalls(1, null, false);
    }

    public override void ModeStopped()
    {
        Game.Logger.Log(GetType().Name + ":" + nameof(ModeStopped), LogLevel.Debug);
        base.ModeStopped();
    }

    public override void UpdateLamps() => Game.Logger.Log(GetType().Name + ":" + nameof(UpdateLamps), LogLevel.Debug);

    /// <summary>
    /// Switch handler for flipper. Default to T
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_flipperLwL_active(Switch sw = null)
    {
        //Game.Logger.Log(GetType().Name + ":" + nameof(sw_flipperLwL_active)+ $": {sw.TimeSinceChange()}", LogLevel.Debug);
        return SWITCH_CONTINUE;
    }

    /// <summary>
    /// HOld delay Switch handler for flipper. Default to T
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_flipperLwL_active_for_1s(Switch sw = null)
    {
        //Game.Logger.Log(GetType().Name + ":" + nameof(sw_flipperLwL_active_for_1s) + $": {sw.TimeSinceChange()}", LogLevel.Debug);
        return SWITCH_CONTINUE;
    }

    public bool sw_slingL_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    public bool sw_slingR_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }
    public bool sw_inlaneL_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    public bool sw_inlaneR_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    public bool sw_outlaneL_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    public bool sw_outlaneR_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    public bool sw_saucer_active_for_1s(Switch sw = null)
    {
        AddPoints(250);
        Game.Coils["saucerEject"].Pulse(40);
        //TODO: start multiball
        return SWITCH_CONTINUE;
    }

    /// <summary>
    /// Start button, starts game and adds a player if the trough is full.
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_start_active(NetProc.Domain.Switch sw)
    {
        //no credits
        if (PinGod.Credits <= 0) return SWITCH_CONTINUE;        

        //TODO: change max players to database
        if(Game.Ball == 1 && Game.Players.Count < 4)
        {
            _game.IncrementAudit("CREDITS_TOTAL", 1);
            _game.IncrementAudit("CREDITS", 1);
            Game.AddPlayer();
            Game.Logger?.Log(nameof(MyMode) + ": player added");
        }        

        return SWITCH_CONTINUE;
    }

    bool OnBumperHit(Switch sw)
    {
        AddPoints(150);
        return SWITCH_CONTINUE;
    }

    bool OnTargetHit(Switch sw)
    {
        AddPoints(200);
        return SWITCH_CONTINUE;
    }

    void AddPoints(int amt) => _game.AddPoints(amt);
}
