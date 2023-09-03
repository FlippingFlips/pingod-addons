using Godot;
using NetProc.Domain;
using PinGod.Core;
using PinGod.Game;
using System;

/// <summary>
/// A P-ROC (PinGodProcMode) but reusing the default PinGod Attract.tscn. <para/>
/// When the mode starts the <see cref="ATTRACT_SCENE"/> is loaded into the tree. <para/> 
/// This mode handles start button to remove this mode and start the game if the P-ROC trough is full.
/// </summary>
public class AttractMode : PinGodProcMode
{
    /// <summary>
    /// attract scene to load when mode starts. The scene is already loaded in the resources, it just gets instantiated and added to the tree.
    /// </summary>
    const string ATTRACT_SCENE = "res://scenes/AttractMode/AttractProc.tscn";

    private PackedScene _attractScene;
    private Node _attractInstance;
    private PinGodGameProc _pingod;

    public AttractMode(IGameController game, int priority, IPinGodGame pinGod, string name = nameof(AttractMode)) : base(game, name, priority, pinGod)
    {
        _pingod = pinGod as PinGodGameProc;
    }

    public override void ModeStarted()
    {
        base.ModeStarted();

        _pingod.CallDeferred("AddModeScene", ATTRACT_SCENE);

        _game.LEDS["start"].Script(
            new NetProc.Domain.Pdb.LEDScript[]{
                new NetProc.Domain.Pdb.LEDScript { Colour = new uint[] { 0xFF, 0x00, 0x00 }, Duration = 500},
                new NetProc.Domain.Pdb.LEDScript { Colour = new uint[] { 0x00, 0x00, 0x00 }, Duration = 500}});
    }

    /// <summary>
    /// removes the attract layer from modes canvas and frees it
    /// </summary>
    public override void ModeStopped()
    {
        base.ModeStopped();
        Logger.Debug(nameof(AttractMode), nameof(ModeStopped));
        if (_attractInstance != null)
        {
            RemoveChildSceneFromCanvasLayer(_attractInstance);
            _attractInstance?.Free();
            _attractInstance = null;
        }
    }

    /// <summary>
    /// Start button, starts game and adds a player if the trough is full. //TODO: BallSearch if no balls when push start
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_start_active(NetProc.Domain.Switch sw)
    {
        //no credits
        if (_pingod.Credits <= 0) return SWITCH_CONTINUE;

        Game.Logger?.Log("start button active");
        if (_game.Trough?.IsFull() ?? false) //todo: credit check?
        {
            _pingod.CallDeferred("StartProcGame");
        }
        else
        {
            Game.Logger?.Log("attract start. trough balls=" + _game.Trough.NumBalls() + ", running ball search.", NetProc.Domain.PinProc.LogLevel.Debug);
            _pingod.PinGodProcGame.BallSearch();

        }
        return SWITCH_CONTINUE;
    }
}
