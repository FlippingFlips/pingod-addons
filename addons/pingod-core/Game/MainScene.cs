using Godot;
using PinGod.Core.Service;
using System;
using System.Threading.Tasks;
using static Godot.GD;

namespace PinGod.Core.Game
{
    /// <summary>
    /// Main Scene. The Entry point
    /// </summary>
    public partial class MainScene : Node
    {
        /// <summary>
        /// Path to the Game.tscn. 
        /// </summary>
        [Export(PropertyHint.File)] protected string _game_scene_path;

        /// <summary>
        /// Path to service menu scene
        /// </summary>
        [Export(PropertyHint.File)] protected string _service_menu_scene_path;

        private MachineNode _machine;
        private Resources _resources;
        private Node attractnode;        

        /// <summary>
        /// Is machine is the Service Menu?
        /// </summary>
        public bool InServiceMenu { get; private set; }
        /// <summary>
        /// PinGodGame singleton
        /// </summary>
        public IPinGodGame pinGod { get; private set; }

        /// <summary>
        /// Connects to <see cref="PinGodBase.GameStartedEventHandler"/>, <see cref="PinGodBase.GameEndedEventHandler"/>, <see cref="PinGodBase.ServiceMenuExitEventHandler"/> <para/>
        /// Holds <see cref="attractnode"/>, <see cref="settingsDisplay"/>, <see cref="pauseLayer"/>
        /// </summary>
        public override void _EnterTree()
        {
            //not working..
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            //PauseMode = PauseModeEnum.Process; //new Godot4 ProcessModeEnum.Always
            //this.ProcessMode = ProcessModeEnum.Always;

            //connect to a switch command. the switches can come from actions or ReadStates
            if (!HasNode("/root/Machine")) 
                Logger.WarningRich(nameof(MainScene), ":[color=yellow]", "/root/Machine node not found. To use switch handling enable pingod-machine plugin[/color]");
            else
            {
                _machine = GetNode<MachineNode>("/root/Machine");
                _machine.SwitchCommand += OnSwitchCommandHandler;
                Logger.Info(nameof(MainScene), ":listening for switches from Machine");
            }
        }

        private void _resources_ResourcesLoaded(int amt)
        {
            Logger.Info(nameof(MainScene), nameof(_resources_ResourcesLoaded), $":{amt}");
            Logger.Debug(nameof(MainScene), ":loaded resources:" + string.Join(',', _resources.GetResourceList()));
            //this.Visible = true; todo
        }

        public override void _Ready()
        {
            base._Ready();

            //load Resources node from PinGodGame
            if (HasNode("/root/Resources"))
            {
                _resources = GetNode("/root/Resources") as Resources;
                _resources.ResourcesLoaded += _resources_ResourcesLoaded;
            }
            else Logger.WarningRich(nameof(MainScene), ":[color=yellow]", "Resources wasn't found in /root/Resources[/color]");

            //save a reference to connect signals
            if (HasNode("/root/PinGodGame"))
                pinGod = GetNode<IPinGodGame>("/root/PinGodGame");
            else Logger.WarningRich(nameof(MainScene), ":[color=yellow]", "PinGodGame not found. Used for GameStart/End, Service Menu events[/color]");

            //game events
            pinGod?.Connect("GameStarted", new Callable(this, nameof(OnGameStarted)));
            pinGod?.Connect("GameEnded", new Callable(this, nameof(OnGameEnded)));
            pinGod?.Connect("ServiceMenuExit", new Callable(this, nameof(OnServiceMenuExit)));

            //attract mod already in the tree, get the instance so we can free it when game started
            attractnode = GetNode("Modes/Attract");

            //this.Visible = false;
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
            if (!string.IsNullOrWhiteSpace(_game_scene_path))
            {
                //load the game scene mode and add to Modes tree		
                LoadSceneMode(_game_scene_path, destroyAttract: true);
            }
            else { Logger.WarningRich(nameof(MainScene), ":", nameof(StartGame), ":[color=yellow]", " A Game scene wasn't provided", "[/color]"); }
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
                    Logger.WarningRich("[color=red]", nameof(MainScene), $": LoadSceneMode, but no scene found at {resourcePath}", "[/color]");
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
                    else { Logger.WarningRich(nameof(MainScene), ":", nameof(OnSwitchCommandHandler), ":[color=yellow]", " A Service menu scene wasn't provided", "[/color]"); }
                }
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

}