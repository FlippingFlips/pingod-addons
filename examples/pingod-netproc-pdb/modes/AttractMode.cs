using Godot;
using NetProc.Domain;
using NetProc.Game.Modes;
using PinGod.Core.Service;
using PinGod.Game;
using System.Linq;

internal class AttractMode : PinGodProcMode
{
    private PackedScene _attractScene;
    private Trough _trough;
    const string ATTRACT_SCENE = "res://addons/modes/attract/Attract.tscn";

    public AttractMode(IGameController game, int priority, PinGodGame pinGod) : base(game, priority, pinGod)
    {

    }

    public override void ModeStarted()
    {
        base.ModeStarted();

        _attractScene = _resources?.GetResource(ATTRACT_SCENE.GetBaseName()) as PackedScene;
        AddChildSceneToCanvasLayer(_attractScene.Instantiate());

        //get the trough from the modes queue
        _trough = Game.Modes.Modes.FirstOrDefault(c => c.GetType() == typeof(Trough)) as Trough;
    }

    public override void ModeStopped()
    {        
        base.ModeStopped();
    }

    public bool sw_start_active(Switch sw)
    {
        Game.Logger?.Log("start button active");
        if (_trough?.IsFull() ?? false)
        {
            Game.Logger.Log("start button, trough full");
            Game.StartGame();
            Game.AddPlayer();
            Game.StartBall();
        }
        else
        {
            Game.Logger?.Log("attract start. trough balls:" + _trough.NumBalls());
            //TODO: Ball search
        }
        return SWITCH_CONTINUE;
    }
}