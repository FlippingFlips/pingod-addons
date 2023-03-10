using Godot;
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
    }

    /// <summary>
    /// Cleans up, removes from canvas and frees this mode. This should usually be when game ends for this mode.
    /// </summary>
    public override void ModeStopped()
    {
        base.ModeStopped();
        if (_scoreDisplay != null)
        {
            RemoveChildSceneFromCanvasLayer(_scoreDisplay);
            _scoreDisplay.QueueFree();            
        }
    }

    public override void ModeStarted()
    {
        if(_resources != null)
        {
            var scene = _resources?.GetResource(SCORE_MODE_SCENE.GetBaseName()) as PackedScene;
            _scoreDisplay = scene?.Instantiate() as ScoreModePROC;
            AddChildSceneToCanvasLayer(_scoreDisplay);
        }
        else { Logger.WarningRich(nameof(ScoreDisplayProcMode), nameof(ModeStarted), ": [color=yellow]no resources found, can't create attract scene[/color]"); }

        base.ModeStarted();
    }

    /// <summary>
    /// Updates the scores in the ScoreMode canvas
    /// </summary>
    internal void UpdateScores() => _scoreDisplay?.OnScoresUpdated();
}