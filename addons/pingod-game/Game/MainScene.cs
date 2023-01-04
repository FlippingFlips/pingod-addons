using Godot;
using System;
using System.Threading.Tasks;
using static Godot.GD;
using static PinGodBase;


/// <summary>
/// Main Scene. The Entry point
/// </summary>
public partial class MainScene : Node2D
{
    /// Emitted signal when game is paused
    /// </summary>
    [Signal] public delegate void GamePausedEventHandler();
    /// <summary>
    /// Emitted signal when game is resumed
    /// </summary>
    [Signal] public delegate void GameResumedEventHandler();

    internal ResourcePreloader _resourcePreLoader;
    /// <summary>
    /// Path to the Game.tscn. 
    /// </summary>
    [Export(PropertyHint.File)] protected string _game_scene_path = "res://Game.tscn";

    /// <summary>
    /// Path to service menu scene
    /// </summary>
    [Export(PropertyHint.File)] protected string _service_menu_scene_path = "res://addons/pingod-servicemenu/ServiceMenu.tscn";

    private Node attractnode;
    Mutex m = new Mutex();
    private Control pauseLayer;
    private Control settingsDisplay;
    private PinGodMachine _machine;

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
        //save a reference to connect signals
        if (HasNode("/root/PinGodGame"))
            pinGod = GetNode<PinGodGame>("/root/PinGodGame");

        //show a pause menu when pause enabled.
        if (this.HasNode("Controls/PauseControl"))
            pauseLayer = GetNode("Controls/PauseControl") as Control;

        if (this.HasNode("Controls/SettingsDisplay"))
            settingsDisplay = GetNodeOrNull<Control>("Controls/SettingsDisplay");

        //TODO: Godot 4 splashtime
        //Logger.Debug(nameof(MainScene), ":splash timer msecs", OS.GetSplashTickMsec());

        //try to catch anything unhandled here, not when ready
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

        //PauseMode = PauseModeEnum.Process; //new Godot4 ProcessModeEnum.Always
        this.ProcessMode = ProcessModeEnum.Always;


        //load Resources node from PinGodGame and load service menu
        if (pinGod?.HasNode("Resources") ?? false)
            _resourcePreLoader = pinGod.GetNode("Resources") as ResourcePreloader;
        else Logger.Warning(nameof(MainScene), $":WARN: no node found in pingod game under Resources");


        PreloadServiceMenu();

        //game events
        pinGod?.Connect(nameof(PinGodGame.GameStarted), new Callable(this, nameof(OnGameStarted)));
        pinGod?.Connect(nameof(PinGodGame.GameEnded), new Callable(this, nameof(OnGameEnded)));
        pinGod?.Connect(nameof(PinGodGame.ServiceMenuExit), new Callable(this, nameof(OnServiceMenuExit)));

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

        //attract mod already in the tree, get the instance so we can free it when game started
        attractnode = GetNode("Modes/Attract");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);        
        if (!@event.IsActionType()) return;
        if (@event is InputEventMouse) return;
        var name = @event.ResourceName;
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
                            GetNode("Modes/Game")?.QueueFree();
                        else
                            GetNode("Modes/Attract")?.Free();

                        //load service menu into modes
                        CallDeferred("_loaded", _resourcePreLoader.GetResource(_service_menu_scene_path.GetBaseName()));

                        pinGod.EmitSignal("ServiceMenuEnter");
                    });
                }
            }
        }
    }

    /// <summary>
    /// End game, reloads the original scene, removing anything added. This could be used as a reset from VP with F3.
    /// </summary>
    public async void OnGameEnded()
    {
        await Task.Run(() =>
        {
            GetNode("Modes/Game").QueueFree();
            CallDeferred(nameof(Reload));
        });
    }

    /// <summary>
    /// When everything is ready, remove the attract, start the game.
    /// </summary>
    public void StartGame()
    {
        //load the game scene mode and add to Modes tree		
        LoadSceneMode(_game_scene_path);
    }

    void _loaded(PackedScene packedScene)
    {
        if (packedScene != null)
        {
            GetNode("Modes").AddChild(packedScene.Instantiate());
            Logger.Debug(nameof(MainScene), ":modes added: ", packedScene.ResourceName);
        }
    }

    /// <summary>
    /// Runs a new task to load a scene and poll until finished. Display freezes in VP without this if scenes are med/large <para/>
    /// <see cref="_loaded(PackedScene)"/>, this will be added a child to the Modes node
    /// </summary>
    private async void LoadSceneMode(string resourcePath)
    {
        await Task.Run(() =>
        {
            m.Lock();
            string name = resourcePath.GetBaseName();
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (_resourcePreLoader == null)
                {
                    Logger.WarningRich("[color=red]", nameof(MainScene),":resource pre-loader is null, skipping load resource", "[/color]");
                    return;
                }

                Resource res = null;
                if (!_resourcePreLoader.HasResource(name))
                {
                    Logger.Debug(nameof(MainScene), ":loading mode scene resource for ", name);
                    res = Load(resourcePath);
                    _resourcePreLoader.AddResource(name, res);
                }
                else
                {
                    Logger.Warning(nameof(MainScene), ":_resourcePreLoader null or scene resource doesn't exists for ", name);
                    //return;
                    res = _resourcePreLoader.GetResource(name);
                }
                CallDeferred("_loaded", res);
            }
            m.Unlock();

            //remove the attract mode
            attractnode.CallDeferred("queue_free");
        });
    }

    /// <summary>
    /// Probably best to use <see cref="LoadSceneMode(string)"/> Runs a new task to load a scene and poll until finished. Display freezes in VP without this if scenes are med/large <para/>
    /// <see cref="_loaded(PackedScene)"/>, this will be added a child to the Modes node
    /// </summary>
    private void LoadSceneModeInteractive(string resourcePath)
    {
        Task.Run(() =>
        {
            m.Lock();
            PackedScene res = ResourceLoader.Load(resourcePath) as PackedScene;
            //PackedScene res; //the resource to return after finished loading
            Logger.Debug(nameof(MainScene), ":loading-" + resourcePath);
            //var total = ril.GetStageCount(); //total resources left to load, can be used progress bar

            //TODO: removed because of Godot4, this was LoadResourceInteractive
            //while (true)
            //{
            //	var err = ril.Poll();
            //	if (err == Error.FileEof)
            //	{
            //		res = ril.GetResource() as PackedScene;
            //		break;
            //	}
            //}

            CallDeferred(nameof(_loaded), res);

            m.Unlock();
        });
    }

    void OnGameStarted()
    {
        CallDeferred(nameof(StartGame));
    }

    void OnServiceMenuExit()
    {
        CallDeferred(nameof(Reload));
        InServiceMenu = false;
    }

    /// <summary>
    /// add the game to preloader
    /// </summary>
    private void PreloadGameScene()
    {
        if (!string.IsNullOrWhiteSpace(_game_scene_path))
        {
            var sMenu = Load(_game_scene_path) as PackedScene;
            _resourcePreLoader?.AddResource(_game_scene_path.GetBaseName(), sMenu);
        }
    }

    /// <summary>
    /// add the service menu scene to preloader
    /// </summary>
    private void PreloadServiceMenu()
    {
        if (!string.IsNullOrWhiteSpace(_service_menu_scene_path))
        {
            var sMenu = Load(_service_menu_scene_path) as PackedScene;
            _resourcePreLoader?.AddResource(_service_menu_scene_path.GetBaseName(), sMenu);
        }
    }

    void Reload() => GetTree().ReloadCurrentScene();

    private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Error($"Unhandled exception {e.ExceptionObject}");
    }
}
