using Godot;
using PinGod.Core;
using PinGod.Core.BallStacks;
using PinGod.Game;
using PinGod.Modes;

/// <summary>
/// Base mode example
/// </summary>
public partial class BaseMode : Control
{
    /// <summary>
    /// Scene to play when ball saves active
    /// </summary>
    [Export(PropertyHint.File)] string BALL_SAVE_SCENE;

    private PackedScene _ballSaveScene;
    private Saucer _ballSaucer;
    private Game game;
    private IPinGodGame pinGod;

    /// <summary>
    /// Gets access to <see cref="PinGodGame"/> and the main <see cref="Game"/> scene. 
    /// </summary>
    public override void _EnterTree()
    {        
        game = GetParent().GetParent() as BasicGame;

        //set the ball save packed scene
        if(!string.IsNullOrWhiteSpace(BALL_SAVE_SCENE)) _ballSaveScene = GD.Load<PackedScene>(BALL_SAVE_SCENE);
        else { Logger.Warning(nameof(BaseMode), ": no ball save scene set"); }

        _ballSaucer = GetNode<Saucer>(nameof(Saucer));               
    }

    public override void _Ready()
    {
        base._Ready();
        if (HasNode("/root/PinGodGame"))
        {
            pinGod = GetNode("/root/PinGodGame") as IPinGodGame;
            //use the switch command on machine through the game as we're in a game
            pinGod.MachineNode.SwitchCommand += OnSwitchCommandHandler;
        }
        else { Logger.WarningRich(nameof(BaseMode), "[color=red]", ": no PinGodGame found", "[/color]"); }
    }

    /// <summary>
    /// Switch handlers for lanes and slingshots
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void OnSwitchCommandHandler(string name, byte index, byte value)
    {
        if (value <= 0) return;
        switch (name)
        {
            case "outlane_l":
            case "outlane_r":
                game.AddPoints(100);
                break;
            case "inlane_l":
            case "inlane_r":
                game.AddPoints(50);
                break;
            case "sling_l":
            case "sling_r":
                game.AddPoints(50);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Not used. Can use to act when ball drains
    /// </summary>
    public void OnBallDrained() { }

    /// <summary>
    /// Displays a ball save scene for 2 seconds if not in multi-ball, <see cref="PinGodGame.IsMultiballRunning"/>
    /// </summary>
    public void OnBallSaved()
    {        
        if (!pinGod.IsMultiballRunning)
        {
            Logger.Debug(nameof(BaseMode),": ball saved, no multi-ball");
            //add ball save scene to tree and remove after 2 secs;
            CallDeferred(nameof(DisplayBallSaveScene), 2f);
        }
        else
        {
            Logger.Debug(nameof(BaseMode),":skipping save display in multi-ball");
        }
    }

    /// <summary>
    /// Does nothing
    /// </summary>
    public void OnBallStarted() { }

    /// <summary>
    /// Does nothing
    /// </summary>
    public void UpdateLamps() { }

    /// <summary>
    /// Adds a ball save scene to the tree and removes
    /// </summary>
    /// <param name="time">removes the scene after the time</param>
    private void DisplayBallSaveScene(float time = 2f)
    {        
        var ballSaveScene = _ballSaveScene?.Instantiate<BallSave>();
        if(ballSaveScene != null)
        {
            Logger.Debug(nameof(BaseMode), ": displaying ball save scene");
            ballSaveScene.SetRemoveAfterTime(time);
            AddChild(_ballSaveScene.Instantiate());
        }
        else { Logger.Debug(nameof(BaseMode), ": ball saver scene not set."); }
    }

    /// <summary>
    /// Saucer "kicker" active.
    /// </summary>
    private void OnBallStackPinball_SwitchActive()
    {
        if (!pinGod.IsTilted && pinGod.GameInPlay)
        {            
            pinGod.AddPoints(150);

            if (!pinGod.IsMultiballRunning)
            {
                Logger.Debug($"{nameof(BaseMode)}:{nameof(OnBallStackPinball_SwitchActive)}", ": starting multiball");
                //enable multiball and start timer on default timeout (see BaseMode scene, BallStackPinball)
                pinGod.IsMultiballRunning = true;
                _ballSaucer.Start();

                game?.CallDeferred(nameof(BasicGame.AddMultiballSceneToTree));
                return;
            }
        }

        //no multiball running or game not in play
        _ballSaucer.Start(1f);
    }

    private void OnBallStackPinball_timeout()
    {
        Logger.Debug(nameof(BaseMode), ":ballstack timedout");
        _ballSaucer.Kick();
    }
}
