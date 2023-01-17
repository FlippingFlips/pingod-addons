using NetProc.Domain;
using NetProc.Domain.PinProc;
using PinGod.Game;
using Switch = NetProc.Domain.Switch;

public partial class MyMode : PinGodProcMode
{
    public MyMode(IGameController game, int priority, PinGodGame pinGod) : base(game, priority, pinGod) { }

    public override void ModeStarted()
    {
        Game.Logger.Log(GetType().Name+":"+nameof(ModeStarted), LogLevel.Debug);
        (Game as PinGodProcGameController).Trough.LaunchBalls(1, null, false);
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
        Game.Logger.Log(GetType().Name + ":" + nameof(sw_flipperLwL_active)+ $": {sw.TimeSinceChange()}", LogLevel.Debug);
        return true;
    }

    /// <summary>
    /// HOld delay Switch handler for flipper. Default to T
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_flipperLwL_active_for_1s(Switch sw = null)
    {
        Game.Logger.Log(GetType().Name + ":" + nameof(sw_flipperLwL_active_for_1s) + $": {sw.TimeSinceChange()}", LogLevel.Debug);
        return true;
    }

    public bool sw_slingL_active(Switch sw = null)
    {
        (Game as PinGodProcGameController).AddPoints(100);
        return true;
    }

    public bool sw_slingR_active(Switch sw = null)
    {
        (Game as PinGodProcGameController).AddPoints(100);
        return true;
    }
}
