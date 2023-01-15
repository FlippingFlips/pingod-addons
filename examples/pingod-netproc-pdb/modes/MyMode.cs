using NetProc.Domain;
using PinGod.Core;
using PinGod.Game;
using Switch = NetProc.Domain.Switch;

public partial class MyMode : PinGodProcMode
{
    public MyMode(IGameController game, int priority, PinGodGame pinGod) : base(game, priority, pinGod) { }

    public override void ModeStarted() => Logger.Info(GetType().Name, ":", nameof(ModeStarted));
    public override void ModeStopped()
    {
        Logger.Info(GetType().Name, ":", nameof(ModeStopped));
        base.ModeStopped();
    }
    public override void UpdateLamps() => Logger.Info(GetType().Name, ":", nameof(UpdateLamps));

    /// <summary>
    /// Switch handler for flipper. Default to T
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_flipperLwL_active(Switch sw = null) 
    {        
        Logger.Info(GetType().Name, ":", nameof(sw_flipperLwL_active), $": {sw.TimeSinceChange()}");
        return true;
    }

    /// <summary>
    /// HOld delay Switch handler for flipper. Default to T
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_flipperLwL_active_for_1s(Switch sw = null)
    {
        Logger.Info(GetType().Name, ":", nameof(sw_flipperLwL_active), $": {sw.TimeSinceChange()}");
        return true;
    }
}
