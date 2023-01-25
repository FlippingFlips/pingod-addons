using Godot;
using PinGod.Core;
using PinGod.Game;
using PinGod.Modes;
using static MoonStation.GameGlobals;

/// <summary>
/// A base mode is a mode that is generally always running.
/// </summary>
public partial class BaseMode : Control
{
    private PackedScene _ballSaveScene;

    /// <summary>
    /// Where our ball scene save is. This is set to use the default from the addons but here to easily be swapped out. * Exported variables can be changed in Godots scene inspector
    /// </summary>
    [Export] string BALL_SAVE_SCENE;
	private MsPinGodGame game;
    private IPinGodGame pinGod;
	public override void _EnterTree()
	{
        game = GetParent().GetParent() as MsPinGodGame;

        //set the ball save packed scene
        if (!string.IsNullOrWhiteSpace(BALL_SAVE_SCENE)) _ballSaveScene = GD.Load<PackedScene>(BALL_SAVE_SCENE);
        else { Logger.Warning(nameof(BaseMode), ": no ball save scene set"); }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (pinGod?.MachineNode != null)
        {
            pinGod.MachineNode.SwitchCommand -= OnSwitchCommandHandler;
        }
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
    private void AddPointsPlaySound(int sMALL_SCORE)
    {
        pinGod.AddPoints(sMALL_SCORE);
        pinGod.AudioManager.PlaySfx("spinner");
    }

    /// <summary>
    /// Adds a ball save scene to the tree and removes
    /// </summary>
    /// <param name="time">removes the scene after the time</param>
    private void DisplayBallSaveScene(float time = 2f)
    {
        var ballSaveScene = _ballSaveScene?.Instantiate<BallSave>();
        if (ballSaveScene != null)
        {
            Logger.Debug(nameof(BaseMode), ": displaying ball save scene");
            ballSaveScene.SetRemoveAfterTime(time);
            AddChild(_ballSaveScene.Instantiate());
        }
        else { Logger.Debug(nameof(BaseMode), ": ball saver scene not set."); }
    }

    /// <summary>
    /// Helper to start MultiBall. Set the game to "IsMultiballRunning", fire the saucer coil to exit ball, add a scene
    /// </summary>
    private void StartMultiball()
    {
        if (!pinGod.IsMultiballRunning)
        {
            pinGod.IsMultiballRunning = true;
            pinGod.SolenoidPulse("mball_saucer");

            //add the multiball scene from the game
            game?.CallDeferred("AddMultiballSceneToTree");
        }
        else
        {
            //already in multiball
            pinGod.SolenoidPulse("mball_saucer");
        }
    }

    /// <summary>
    /// simple add points and play a single sound
    /// </summary>
    /// <param name="swName"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void OnSwitchCommandHandler(string swName, byte index, byte value)
    {
        if (!pinGod.GameInPlay || pinGod.IsTilted) return;
        if (value > 0)
		{
            switch (swName)
			{
				case "inlaneL":					
                case "inlaneR":
                case "bumper0":
                case "bumper1":
                    AddPointsPlaySound(SMALL_SCORE);
                    break;
                case "outlaneL":
                case "outlaneR":
                    AddPointsPlaySound(MED_SCORE);
                    break;
                case "slingL":
                case "slingR":
                case "spinner":
                    AddPointsPlaySound(MIN_SCORE);
                    break;
				case "top_left_target":
                    AddPointsPlaySound(LARGE_SCORE);
                    break;
                default:
					break;
			}
		}		
    }
	#region Mode Group Methods

	//This mode is added to a group named Group in the BaseMode scene
	//These methods will be called by the game if the the scene is in the "Mode" group

	public void OnBallDrained() { }

	public void OnBallSaved()
	{
        if (!pinGod.IsMultiballRunning)
        {
            Logger.Debug(nameof(BaseMode), ": ball saved, no multi-ball");
            //add ball save scene to tree and remove after 2 secs;
            CallDeferred(nameof(DisplayBallSaveScene), 2f);
        }
        else
        {
            Logger.Debug(nameof(BaseMode), ":skipping save display in multi-ball");
        }
    }

	/// <summary>
	/// When ball is started
	/// </summary>
	public void OnBallStarted()
	{
		pinGod?.LogInfo("game: ball started");
		if (pinGod?.AudioManager?.MusicEnabled ?? false)
		{
			pinGod.AudioManager.PlayMusic(pinGod.AudioManager.Bgm);
		}
	}
	public void UpdateLamps() { }
	#endregion
}
