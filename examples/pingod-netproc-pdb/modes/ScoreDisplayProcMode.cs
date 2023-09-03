﻿using Godot;
using NetProc.Domain;
using PinGod.Core;
using PinGod.Game;
using PinGod.Modes;

/// <summary>
/// A P-ROC (PinGodProcMode) but reusing the default PinGod ScoreDisplay
/// </summary>
public class ScoreDisplayProcMode : PinGodProcMode
{
    private ScoreModePROC _scoreDisplay;
    private PinGodGameProc _pingod;
    private Node _sceneInstance;

    /// <summary>
    /// Custom class of <see cref="ScoreMode"/>. This needs to be custom so we can use the P-ROC game controller players.
    /// </summary>
    const string SCORE_MODE_SCENE = "res://scenes/ScoreMode/ScoreModePROC.tscn";

    /// <summary>
    /// Gets the score display scene from Resources and adds it to the Godot tree
    /// </summary>
    /// <param name="game"></param>
    /// <param name="priority"></param>
    /// <param name="pinGod"></param>
    /// <param name="defaultScene"></param>
    /// <param name="loadDefaultScene"></param>
    public ScoreDisplayProcMode(IGameController game, PinGodGame pinGod, string name = nameof(ScoreDisplayProcMode), int priority = 1, string defaultScene = null, bool loadDefaultScene = true) 
        : base(game, name, priority, pinGod, defaultScene, loadDefaultScene)
    {
        _pingod = pinGod as PinGodGameProc;
    }

    /// <summary>
    /// Cleans up, removes from canvas and frees this mode. This should usually be when game ends for this mode.
    /// </summary>
    public override void ModeStopped()
    {
        base.ModeStopped();
        if (_sceneInstance != null)
        {
            RemoveChildSceneFromCanvasLayer(_sceneInstance);         
        }
    }
    
    public override void ModeStarted()
    {
        base.ModeStarted();

        var variant = _pingod.CallDeferred("AddModeScene", SCORE_MODE_SCENE);

        _sceneInstance = variant.As<Node>();

        if(_sceneInstance == null) {
            Game.Logger.Log("SCORE MODE SCENE NOT INSTANCED", NetProc.Domain.PinProc.LogLevel.Error);
        }
    }

    /// <summary>
    /// Updates the scores in the ScoreMode canvas
    /// </summary>
    internal void UpdateScores()
    {
        //_sceneInstance?
        //.CallDeferred("OnScoresUpdated");

        _pingod.CallDeferred("emit_signal", "ScoresUpdated");
    }
}