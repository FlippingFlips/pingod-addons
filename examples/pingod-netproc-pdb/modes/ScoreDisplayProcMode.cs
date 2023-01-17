using Godot;
using NetProc.Domain;
using PinGod.Game;
using PinGod.Modes;

/// <summary>
/// A P-ROC (PinGodProcMode) but reusing the default PinGod ScoreDisplay
/// </summary>
public class ScoreDisplayProcMode : PinGodProcMode
{
    private ScoreMode _scoreDisplay;

    /// <summary>
    /// The default score mode with its script. add this to the layers
    /// </summary>
    const string SCORE_MODE_SCENE = "res://addons/modes/scoremode/ScoreMode.tscn";

    /// <summary>
    /// Gets the score display scene from Resources and adds it to the Godot tree
    /// </summary>
    /// <param name="game"></param>
    /// <param name="priority"></param>
    /// <param name="pinGod"></param>
    /// <param name="defaultScene"></param>
    /// <param name="loadDefaultScene"></param>
    public ScoreDisplayProcMode(IGameController game, int priority, PinGodGame pinGod, string defaultScene = null, bool loadDefaultScene = true) : base(game, priority, pinGod, defaultScene, loadDefaultScene)
    {
        var res = _resources?.GetResource(SCORE_MODE_SCENE.GetBaseName()) as PackedScene;
        if(res != null)
        {
            _scoreDisplay = res.Instantiate() as ScoreMode;
            if (_scoreDisplay != null)
            {
                AddChildSceneToCanvasLayer(_scoreDisplay);
            }
        }
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

    public override void ModeStarted() => base.ModeStarted();
}