using Godot;
using System;
using System.Threading.Tasks;
using static Godot.GD;


/// <summary>
/// Main Scene. The Entry point
/// </summary>
public partial class MainScene : Node2D
{
    /// <summary>
    /// Path to the Game.tscn. 
    /// </summary>
    [Export(PropertyHint.File)] protected string _game_scene_path = "res://Game.tscn";

    /// <summary>
    /// Path to service menu scene
    /// </summary>
    [Export(PropertyHint.File)] protected string _service_menu_scene_path = "res://addons/pingod-mode-servicemenu/ServiceMenu.tscn";

    private PinGodMachine _machine;
    private Resources _resources;
    private Node attractnode;

    private Control pauseLayer;

    private Control settingsDisplay;

    /// Emitted signal when game is paused
    /// </summary>
    [Signal] public delegate void GamePausedEventHandler();
    /// <summary>
    /// Emitted signal when game is resumed
    /// </summary>
    [Signal] public delegate void GameResumedEventHandler();
    /// <summary>
    /// Is machine is the Service Menu?
    /// </summary>
    public bool InServiceMenu { get; private set; }
    /// <summary>
    /// PinGodGame singleton
    /// </summary>
    public PinGodGame pinGod { get; private set; }

    /// <summary>
    /// Connects to <see cref="PinGodBase.GameStartedEventHandler"/>, <see cref="PinGodBase.GameEndedEventHandler"/>, <see cref="PinGodBase.ServiceMenuExitEventHandler"/> <para/>
    /// Holds <see cref="attractnode"/>, <see cref="settingsDisplay"/>, <see cref="pauseLayer"/>
    /// </summary>
    public override void _EnterTree()
    {
        //show a pause menu when pause enabled.
        if (this.HasNode("Controls/PauseControl"))
            pauseLayer = GetNode("Controls/PauseControl") as Control;
        //settings display
        if (this.HasNode("Controls/SettingsDisplay"))
            settingsDisplay = GetNodeOrNull<Control>("Controls/SettingsDisplay");

        //not working..
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

        //PauseMode = PauseModeEnum.Process; //new Godot4 ProcessModeEnum.Always
        this.ProcessMode = ProcessModeEnum.Always;

        //load Resources node from PinGodGame
        if (HasNode("/root/Resources"))
        {
            _resources = GetNode("/root/Resources") as Resources;
        }            
        else Logger.Warning(nameof(MainScene), $":WARN: no node found in pingod game under Resources");

        //connect to a switch command. the switches can come from actions or ReadStates
        if (!HasNode("/root/Machine"))
        {
            Logger.WarningRich(nameof(MainScene), ":[color=yellow]", "/root/Machine node not found. To use switch handling enable pingod-machine plugin", "[/color]");
        }
        else
        {
            _machine = GetNode<PinGodMachine>("/root/Machine");
            _machine.SwitchCommand += OnSwitchCommandHandler;
            Logger.Info(nameof(MainScene), ":listening for switches from Machine");
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);        
        if (!@event.IsActionType()) return;
        if (@event is InputEventMouse) return;
        
        //todo: move all of this into window actions
        if (InputMap.HasAction("settings"))
        {
            if (@event.IsActionPressed("settings"))
            {
                if (settingsDisplay != null)
                {
                    var visible = !settingsDisplay.Visible;
                    settingsDisplay.Visible = visible;
                    if (visible)
                    {
                        if (!GetTree().Paused)
                            OnPauseGame();
                    }
                    else
                    {
                        OnResumeGame();
                    }                    
                }
            }
        }

        if (InputMap.HasAction("pause"))
        {
            if (@event.IsActionPressed("pause"))
            {
                //if (!settingsDisplay?.Visible ?? false)
                if (!settingsDisplay?.Visible ?? false)
                {
                    if (pauseLayer.Visible) OnResumeGame(); else OnPauseGame();
                }                
                return;
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();        

        //save a reference to connect signals
        if (HasNode("/root/PinGodGame"))
            pinGod = GetNode<PinGodGame>("/root/PinGodGame");

        //game events
        pinGod?.Connect(nameof(PinGodGame.GameStarted), new Callable(this, nameof(OnGameStarted)));
        pinGod?.Connect(nameof(PinGodGame.GameEnded), new Callable(this, nameof(OnGameEnded)));
        pinGod?.Connect(nameof(PinGodGame.ServiceMenuExit), new Callable(this, nameof(OnServiceMenuExit)));

        //attract mod already in the tree, get the instance so we can free it when game started
        attractnode = GetNode("Modes/Attract");          
    }

    /// <summary>
    /// End game, reloads the original scene, removing anything added. This could be used as a reset from VP with F3.
    /// </summary>
    public async virtual void OnGameEnded()
    {
        await Task.Run(() =>
        {
            GetNode("Modes/Game").QueueFree();
            CallDeferred(nameof(Reload));
        });
    }

    /// <summary>
    /// Calls <see cref="OnGameEnded"/>, removes the game played and calls <see cref="Reload"/>
    /// </summary>
    public virtual void ResetGame() => OnGameEnded();

    /// <summary>
    /// When everything is ready, remove the attract, start the game.
    /// </summary>
    public virtual void StartGame()
    {
        //load the game scene mode and add to Modes tree		
        LoadSceneMode(_game_scene_path, destroyAttract:true);  
    }

    /// <summary>
    /// adds the loaded scene as a child of the "Modes" node
    /// </summary>
    /// <param name="packedScene"></param>
    void _loaded(PackedScene packedScene)
    {
        if (packedScene != null)
        {
            GetNode("Modes").AddChild(packedScene.Instantiate());
            Logger.Debug(nameof(MainScene), ":modes added: ", packedScene.ResourceName);
        }
    }

    /// <summary>
    /// Loads a scene and adds to the Modes (Node) in this scene if set to do so. <para/>
    /// It's best if these scenes are already loaded into the preloaded by using the packed scenes in Resources.tscn (autoload). <para/>
    /// Example: BasicGame uses service mode and Game.tscn preloaded, so when they come here it is getting a resource that was already loaded.
    /// </summary>
    /// <param name="resourcePath"></param>
    /// <param name="addToModesOnLoad"></param>
    /// <param name="destroyAttract"></param>
    private void LoadSceneMode(string resourcePath, bool addToModesOnLoad = true, bool destroyAttract = false)
    {
        if (_resources == null)
        {
            Logger.WarningRich("[color=red]", nameof(MainScene), ":no /root/Resources, skipping load resource", "[/color]");
            return;
        }

        //name without the .tscn for some reason
        string name = resourcePath.GetBaseName();

        Resource res = null;
        if (_resources.HasResource(name))
        {
            //already loaded
            res = _resources.GetResource(name);
        }
        else
        {
            //load from path
            if (!FileAccess.FileExists(resourcePath))
            {
                Logger.WarningRich("[color=red]", nameof(MainScene), $": LoadScene but no resource found at {resourcePath}", "[/color]");
                return;
            }

            Logger.Debug(nameof(MainScene), ":loading mode scene resource for ", name);
            res = Load(resourcePath);
            _resources.AddResource(name, res);
        }

        //
        if (res == null)
        {
            Logger.Warning(nameof(MainScene), ":_resourcePreLoader null or scene resource doesn't exists for ", name);
        }
        else
        {
            if (addToModesOnLoad)
                CallDeferred("_loaded", res);
        }

        if (destroyAttract)
        {
            if (!attractnode.IsQueuedForDeletion())
            {
                //remove the attract mode
                attractnode.QueueFree();
            }
        }
    }

    void OnGameStarted()
    {
        CallDeferred(nameof(StartGame));
    }

    private void OnPauseGame()
    {
        Logger.Debug(nameof(PinGodGame), ":pause");
        GetNode("/root").GetTree().Paused = true;
        //settingsDisplay.GetTree().Paused = false;        
        pauseLayer.Show();
    }

    private void OnResumeGame()
    {
        Logger.Debug(nameof(PinGodGame), ":resume");
        pauseLayer.Hide();

        //GetTree().Paused = false;
        GetNode("/root").GetTree().Paused = false;
    }

    void OnServiceMenuExit()
    {
        CallDeferred(nameof(Reload));
        InServiceMenu = false;
    }

    private void OnSwitchCommandHandler(string name, byte index, byte value)
    {
        if (value <= 0) return;
        if ((!pinGod?.IsTilted ?? true) && name == "enter")
        {
            if (!InServiceMenu)
            {
                if (!string.IsNullOrWhiteSpace(_service_menu_scene_path))
                {
                    //enter service menu					
                    InServiceMenu = true;

                    Task.Run(() =>
                    {
                        if (pinGod.GameInPlay)
                            GetNodeOrNull("Modes/Game")?.QueueFree();
                        else
                            GetNodeOrNull("Modes/Attract")?.QueueFree();

                        //load service menu into modes
                        CallDeferred("_loaded", _resources?.GetResource(_service_menu_scene_path.GetBaseName()));

                        pinGod.EmitSignal("ServiceMenuEnter");
                    });
                }
            }
        }
    }

    private void Pause()
    {
        //if (!settingsDisplay?.Visible ?? false)
        if (GetTree().Paused)
        {
            Logger.Debug(nameof(MainScene), ":resume");
            //SetGameResumed();
            //pauseLayer.Hide();
            EmitSignal(nameof(GameResumed));
            GetTree().Paused = false;
        }
        else
        {
            //OnPauseGame();
            EmitSignal(nameof(GamePaused));
            GetTree().Paused = false;
        }
    }


    /// <summary>
    /// Reload the current scene, the main scene.
    /// </summary>
    void Reload() => GetTree().ReloadCurrentScene();

    private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Error($"Unhandled exception {e.ExceptionObject}");
    }
}
