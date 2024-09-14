using Godot;
using PinGod.Base;
using PinGod.EditorPlugins;

/// <summary>A Base mode that handles displaying a ball save when triggered</summary>
public partial class BaseMode : PinGodGameModeControl
{
    #region Exports
    /// <summary> Scene to play when ball save becomes active </summary>
    [Export(PropertyHint.File)] string BALL_SAVE_SCENE;
    [Export(PropertyHint.File)] string DISPLAY_MSG_SCENE;
    #endregion

    #region Fields
    private PackedScene _ballSaveScene;
    private IGame _demoGame;
    private Saucer _ballSaucer;
    private Spinner _spinner;
    private TargetsBank _targets;
    private PackedScene _displayMsgScene;
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
        _demoGame = GetParent().GetParent() as DemoGameNode;

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

        //add a display message to instance if available in resources
        if (!string.IsNullOrWhiteSpace(DISPLAY_MSG_SCENE))
        {
            if (_resources != null)
            {
                _displayMsgScene = _resources.GetPackedSceneFromResource(DISPLAY_MSG_SCENE);

                if (_displayMsgScene == null) { Logger.Error(nameof(BaseMode), $":{DISPLAY_MSG_SCENE}: the scene hasn't been added to the resources."); }
            }
            else { Logger.Warning(nameof(BaseMode), ": no resources found."); }
        }
    }

    /// <summary> Hooks onto the PinGodGame.Machine switch handler where we process our switches</summary>
    public override void _Ready()
    {
        base._Ready();

        //hook up the switch handler from the machine
        if (_pinGod != null)
            _pinGod.MachineNode.SwitchCommand += OnSwitchCommandHandler;
        else { Logger.Log(LogLevel.Error, Logger.BBColor.red, nameof(BaseMode), "[color=red]", ": no PinGodGame found", "[/color]"); }

        //get the ball saucer from the scene
        _ballSaucer = GetNodeOrNull<Saucer>(nameof(Saucer));
        //get the spinner from the scene
        _spinner = GetNodeOrNull<Spinner>(nameof(Spinner));
        //get the targets from the scene
        _targets = GetNodeOrNull<TargetsBank>(nameof(TargetsBank));
        _targets.UpdateLamps();
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

    /// <summary>Start a multi-ball when player hits the saucer if it's not already running</summary>
    void OnSaucerSwitchActive()
    {
        if(!_pinGod?.IsMultiballRunning ?? false && _pinGod?.BallsInPlay() == 1)
        {
            _pinGod.AddPoints(1000);
            _pinGod.AddBonus(500);

            //start the timer on the saucer to kick the ball
            _ballSaucer?.Start(2);

            Logger.Log(LogLevel.Warning, Logger.BBColor.green,
            nameof(OnSaucerSwitchActive), ": starting multi-ball");

            //get the DemoGame node, not the PinGodGame
            var demogame = _demoGame as DemoGameNode;

            //run the add multi-ball scene on the game
            demogame.CallDeferred(nameof(demogame.AddMultiballSceneToTree));

            return;
        }
        else 
        {
            Logger.Log(LogLevel.Warning, Logger.BBColor.green,
            nameof(OnSaucerSwitchActive), ": multi-ball is already running!");
            _ballSaucer.Start(2);
        }
    }

    /// <summary>Kicks a ball on time out</summary>
    void OnSaucerTimeOut()
    {
        Logger.Debug(nameof(BaseMode), ":saucer timed out, kicking ball...");

        _ballSaucer?.Kick();
    }

    /// <summary>This is hooked up to the TargetsBank node in the scene<para/>
    /// When the target switches are all completed this will be fired</summary>
    void OnTargetsBankCompleted()
    {
        Logger.Debug(nameof(BaseMode), ":TARGETS COMPLETED");

        _pinGod.PlaySfx("warning");
        _pinGod.AddPoints(1000);
        _pinGod.AddBonus(500);

        //create an instance of message layer
        var scene = _displayMsgScene?.Instantiate() as DisplayMessageControl;

        //show targets complete
        scene.SetText("TARGETS\nCOMPLETED");

        //time to show the scene for
        scene.SetTime(2);
        this.AddChild(scene);
    }

    void OnTargetSwitchActive(string name, bool complete)
    {
        Logger.Debug(nameof(BaseMode), $":{nameof(OnTargetSwitchActive)}: {name}-{complete}");
        if(complete)
        {
            _pinGod.AddPoints(150);
            _pinGod.AddBonus(50);
        }        
    }

    void OnSpinnerActive(int state)
    {
        if (state > 0) { _pinGod.AddPoints(250); }
    }
}
