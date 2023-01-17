﻿using Godot;
using NetProc.Domain;
using PinGod.Core;

/// <summary>
/// A P-ROC (PinGodProcMode) but reusing the default PinGod Attract.tscn. <para/>
/// When the mode starts the <see cref="ATTRACT_SCENE"/> is loaded into the tree. <para/> 
/// This mode handles start button to remove this mode and start the game if the P-ROC trough is full.
/// </summary>
internal class AttractMode : PinGodProcMode
{
    /// <summary>
    /// attract scene to load when mode starts. The scene is already loaded in the resources, it just gets instantiated and added to the tree.
    /// </summary>
    const string ATTRACT_SCENE = "res://addons/modes/attract/Attract.tscn";

    private PackedScene _attractScene;
    private Node _attractInstance;
    private PinGodProcGameController _game;

    public AttractMode(IGameController game, int priority, IPinGodGame pinGod) : base(game, priority, pinGod) 
    {
        _game = Game as PinGodProcGameController;
    }

    public override void ModeStarted()
    {
        base.ModeStarted();

        //get the pre loaded resource, create instance and add to base mode canvas
        _attractScene = _resources?.GetResource(ATTRACT_SCENE.GetBaseName()) as PackedScene;
        _attractInstance = _attractScene.Instantiate();
        AddChildSceneToCanvasLayer(_attractInstance);
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
        Game.Logger?.Log("start button active");
        if (_game.Trough?.IsFull() ?? false) //todo: credit check?
        {
            Game.Logger.Log("start button, trough full");
            Game.StartGame();
            Game.AddPlayer();
            Game.StartBall();
            Game.Modes.Remove(this);
        }
        else
        {
            Game.Logger?.Log("attract start. trough balls:" + _game.Trough.NumBalls());            
        }
        return SWITCH_CONTINUE;
    }
}