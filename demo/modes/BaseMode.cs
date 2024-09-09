using Godot;
using PinGod.Base;

/// <summary>A Base mode that handles displaying a ball save when triggered</summary>
public partial class BaseMode : PinGodGameModeControl
{
    #region Exports
    /// <summary> Scene to play when ball save becomes active </summary>
    [Export(PropertyHint.File)] string BALL_SAVE_SCENE;
    #endregion

    #region Fields
    private PackedScene _ballSaveScene;
    private IGame _demoGame;
    #endregion

    #region Godot Overrides
    /// <summary>Initializes pingodgame and resources from the base and loads the ball save scene from the resources<para/>
    /// We also get a reference to our DemoGame</summary>
    public override void _EnterTree()
    {
        //call the base to get the singletons
        base._EnterTree();

        //get a reference to the game, above the modes control
        //use this to add points with our method on the DemoGame
        _demoGame = GetParent().GetParent() as DemoGame;

        //add a ball save scene to instance if available in resources
        if (!string.IsNullOrEmpty(BALL_SAVE_SCENE))
        {
            if (_resources != null)
            {
                _ballSaveScene = _resources.GetPackedSceneFromResource(BALL_SAVE_SCENE);
                if (_ballSaveScene == null) { Logger.Error(nameof(BaseMode), ": the scene hasn't been added to the resources."); }
            }
            else { Logger.Warning(nameof(BaseMode), ": no resources found."); }
        }
        else { Logger.Warning(nameof(BaseMode), ": no ball save scene set..."); }
    }

    /// <summary> Hooks onto the PinGodGame.Machine switch handler where we process our switches</summary>
    public override void _Ready()
    {
        base._Ready();
        if (_pinGod != null)
            _pinGod.MachineNode.SwitchCommand += OnSwitchCommandHandler;
        else { Logger.Log(LogLevel.Error, Logger.BBColor.red, nameof(BaseMode), "[color=red]", ": no PinGodGame found", "[/color]"); }
    }

    #endregion

    /// <summary>Shows the ball save scene by adding an instance of the packed scene as a child in the tree.<para/>
    /// This is method triggered by the game being in the Group name Mode</summary>
    protected override void OnBallSaved()
    {        
        if( _ballSaveScene != null)
        {
            Logger.Log(LogLevel.Info, Logger.BBColor.green,
                nameof(BaseMode), $":{nameof(OnBallSaved)}");

            AddChild(_ballSaveScene.Instantiate());
        }            
    }

    /// <summary>Switch handlers for lanes, slingshots and bumpers.<para/>
    /// This example uses a switch case to add points when switches are active.</summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void OnSwitchCommandHandler(string name, byte index, byte value)
    {
        if (value <= 0) return;
        switch (name)
        {
            case "outlaneL":
            case "outlaneR":
                _demoGame.AddPoints(250);
                break;
            case "inlaneL":
            case "inlaneR":
                _demoGame.AddPoints(100);
                break;
            case "slingL":
            case "slingR":
                _demoGame.AddPoints(100);
                break;
            case "bumper1":
            case "bumper2":
            case "bumper3":
            case "bumper4":
                _demoGame.AddPoints(50);
                break;
            default:
                break;
        }
    }
}
