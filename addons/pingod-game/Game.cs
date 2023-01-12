using Godot;
using PinGod.Core;
using PinGod.Core.Service;
using PinGod.Modes;
using System.Reflection;

namespace PinGod.Game
{
    /// <summary>
    /// Custom Game class for Basic Game
    /// </summary>
    public partial class Game : PinGodGameNode, IGame
    {
        public Tilt tilt;

        [ExportCategory("Mode Scenes")]
        [Export(PropertyHint.File)] protected string BONUS_SCENE;
        /// <summary>
        /// Default scene file to use for Multi-Ball
        /// </summary>
        [Export(PropertyHint.File)] protected string MULTIBALL_SCENE;
        [Export(PropertyHint.File)] protected string SCORE_ENTRY_SCENE;
        [Export(PropertyHint.File)] protected string TILT_SCENE;

        private bool _lastBall;
        private CanvasLayer _modesUpper;
        private Resources _resources;
        private Timer _tiltedTimeOut;
        private PackedScene multiballPkd;

        /// <summary>
        /// Connects signals to basic game events, handles tilt time outs
        /// </summary>
        public override void _EnterTree()
        {
            base._EnterTree();
            Logger.Debug(nameof(Game), ":", nameof(_EnterTree), ":looking for ScoreEntry and Bonus scenes..");

            //modes upper canvas.
            _modesUpper = GetNodeOrNull<CanvasLayer>("Modes-Upper");            

            Logger.Debug(nameof(Game), ":loading multiball scene:  " + MULTIBALL_SCENE);
            //multiballPkd = ResourceLoader.Load(MULTIBALL_SCENE) as PackedScene; //TODO packed scene

            if (pinGod == null)
            {
                Logger.Warning(nameof(Game), ": No PinGodGame found in root");
            }

            Logger.Debug(nameof(Game), ":", nameof(_EnterTree), ":creating tilt timeout");
            _tiltedTimeOut = new Timer() { OneShot = true, WaitTime = 4, Autostart = false };
            AddChild(_tiltedTimeOut);
            _tiltedTimeOut.Connect("timeout", new Callable(this, nameof(timeout)));
        }

        /// <summary>
        /// Removes event handlers for pingod game events
        /// </summary>
        public override void _ExitTree()
        {
            if (pinGod != null)
            {
                var pg = pinGod as PinGodGame;
                pg.BallDrained -= OnBallDrained;
                pg.BallEnded -= OnBallEnded;
                pg.BallSaved -= OnBallSaved;
                pg.BonusEnded -= OnBonusEnded;
                pg.MultiBallEnded -= EndMultiball;
            }
            _modesUpper.GetNodeOrNull<Tilt>(nameof(Tilt))?.QueueFree();
            Logger.Debug(nameof(Game), ":", nameof(_ExitTree));
            base._ExitTree();            
        }

        /// <summary>
        /// Starts a new game and ball (soon as this Game enters the scene tree). OnBallStarted is invoked on nodes grouped as Mode
        /// </summary>
        public override void _Ready()
        {
            if (pinGod != null)
            {
                var pg = pinGod as PinGodGame;
                pg.BallDrained += OnBallDrained;
                pg.BallEnded += OnBallEnded;
                pg.BallSaved += OnBallSaved;
                pg.BonusEnded += OnBonusEnded;
                pg.MultiBallEnded += EndMultiball;
                //pinGod.PlayerAdded += OnPlayerAdded;
            }

            if (HasNode("/root/Resources")) _resources = GetNodeOrNull<Resources>("/root/Resources");

            //TILT
            LoadAndStartTiltMode();

            Logger.Debug(nameof(Game), ":", nameof(_Ready));
            pinGod.BallInPlay = 1;
            StartNewBall();
        }

        /// <summary>
        /// Gets an instance of the multi-ball scene and add it to the Modes tree in group named `multi-ball`
        /// </summary>
        public virtual void AddMultiballSceneToTree()
        {
            //create an mball instance from the packed scene
            var mball = multiballPkd.Instantiate();
            //add to multiball group
            mball.AddToGroup("multi-ball");
            //add to the tree
            GetNode("Modes").AddChild(mball);
        }
        /// <summary>
        /// add points with 25 bonus
        /// </summary>
        /// <param name="points"></param>
        /// <param name="bonus"></param>
        public virtual void AddPoints(int points, int bonus = 25)
        {
            pinGod.AddPoints(points);
            pinGod.AddBonus(bonus);
        }

        /// <summary>
        /// Sets <see cref="PinGodGame.IsMultiballRunning"/> to false and Any node that is in the multiball group is removed from tree
        /// </summary>
        public virtual void EndMultiball()
        {
            Logger.Debug(nameof(Game), ":removing multi-balls. sending EndMultiball to multi-ball group");
            GetTree().CallGroup("multi-ball", "EndMultiball");
            pinGod.IsMultiballRunning = false;
        }

        /// <summary>
        /// Load the <see cref="BONUS_SCENE"/> from the <see cref="Resources"/>. Creates and instance and adds to the Modes-Upper <para/>
        /// This scene is clear when finished. Override this if you're not using or to add to it. The <see cref="BONUS_SCENE"/> can be changed in the Game inspector
        /// </summary>
        public virtual void LoadAndStartBonusMode()
        {
            var scene = _resources.GetPackedSceneFromResource(BONUS_SCENE);
            if (scene != null)
            {
                Logger.Info(nameof(Game), ":adding bonus scene for player: " + pinGod.CurrentPlayerIndex);
                pinGod.InBonusMode = true;
                var bonus = scene.Instantiate() as Bonus;
                _modesUpper.AddChild(bonus);
                bonus.StartBonusDisplay();                
            }

            if (!string.IsNullOrWhiteSpace(BONUS_SCENE))
            {
                Logger.Info(nameof(Game), ":adding bonus scene for player: " + pinGod.CurrentPlayerIndex);
                pinGod.InBonusMode = true;
                var bonusScene = _resources?.GetResource(BONUS_SCENE.GetBaseName()) as PackedScene;
                if (bonusScene != null)
                {
                    var bonus = bonusScene.Instantiate() as Bonus;
                    _modesUpper.AddChild(bonus);
                    bonus.StartBonusDisplay();
                }
                else { Logger.Warning(nameof(Game), ": no bonus scene could be created. Is the scene added to resources and the scene packed scene is set here?"); }
            }
            else { Logger.WarningRich(nameof(Game), "[color=yellow]", " OnBallEnded: No Bonus scene set", "[/color]"); }
        }

        /// <summary>
        /// Load the <see cref="SCORE_ENTRY_SCENE"/> from the <see cref="Resources"/>. Creates and instance and adds to the Modes-Upper <para/>
        /// This scene is clear when finished. Override this if you're not using or to add to it. The <see cref="SCORE_ENTRY_SCENE"/> can be changed in the Game inspector
        /// </summary>
        public virtual void LoadAndStartScoreEntryMode()
        {
            var scene = _resources.GetPackedSceneFromResource(SCORE_ENTRY_SCENE);
            if (scene != null)
            {
                Logger.Info(nameof(Game), ":adding score entry scene");
                var scoreEntry = scene.Instantiate() as ScoreEntry;
                _modesUpper.AddChild(scoreEntry);
                scoreEntry.ScoreEntryEnded += OnScoreEntryEnded;
                scoreEntry?.DisplayHighScore();
            }
        }

        public virtual void LoadAndStartTiltMode()
        {
            var scene = _resources.GetPackedSceneFromResource(TILT_SCENE);
            if (scene != null)
            {
                Logger.Info(nameof(Game), ":adding tilt scene");
                var tilt = scene.Instantiate() as Tilt;
                _modesUpper.AddChild(tilt);
            }
        }

        /// <summary>
        /// Invokes <see cref="PinGodGame.OnBallDrained(SceneTree, string, string)"/> on the whole tree
        /// </summary>
        public virtual void OnBallDrained()
        {
            if (_tiltedTimeOut.IsStopped())
            {
                if (pinGod != null)
                {
                    pinGod.OnBallDrained(GetTree());

                    if (pinGod.EndBall())
                    {
                        Logger.Info(nameof(Game), ":last ball played, game ending");
                    }
                    else
                    {
                        Logger.Info(nameof(Game), ":new ball starting");
                    }
                }
            }
        }

        /// <summary>
        /// Add a display at end of ball
        /// </summary>
        public virtual void OnBallEnded(bool lastBall)
        {
            Logger.Info(nameof(Game), ":ball ended", pinGod.BallInPlay, " last ball:" + lastBall);
            _lastBall = lastBall;

            EndMultiball();

            if (!pinGod.IsTilted)
            {
                LoadAndStartBonusMode();
                return;
            }
            else if (pinGod.IsTilted && lastBall)
            {
                Logger.Debug(nameof(Game), ":last ball in tilt");
                pinGod.InBonusMode = false;
                LoadAndStartScoreEntryMode();
                return;
            }
            else if (pinGod.IsTilted)
            {
                if (_tiltedTimeOut.IsStopped())
                {
                    pinGod.InBonusMode = false;
                    Logger.Info(nameof(Game), ":no bonus, game was tilted. running timer to make player wait");
                    _tiltedTimeOut.Start(4);
                }
                else
                {
                    Logger.Debug(nameof(Game), ":still tilted");
                }
            }
        }

        /// <summary>
        /// Signals to Mode groups OnBallSaved
        /// </summary>
        public virtual void OnBallSaved() => pinGod.OnBallSaved(GetTree());

        /// <summary>
        /// Displays high score if this is the last ball.
        /// </summary>
        public virtual void OnBonusEnded()
        {
            Logger.Debug(nameof(Game), ":bonus ended, starting new ball");
            pinGod.InBonusMode = false;
            if (_lastBall)
            {
                Logger.Debug(nameof(Game), ":last ball played, end of game");
                LoadAndStartScoreEntryMode();
            }
            else
            {
                StartNewBall();
                pinGod.UpdateLamps(GetTree());
            }
        }

        /// <summary>
        /// When score entry is finished set <see cref="PinGodGame.EndOfGame"/>
        /// </summary>
        public virtual void OnScoreEntryEnded()
        {
            Logger.Debug(nameof(Game), " ", nameof(OnScoreEntryEnded));
            pinGod.EndOfGame();
        }

        /// <summary>
        /// Just before <see cref="StartNewBall"/> is called
        /// </summary>
        public virtual void OnStartNewBall()
        {
            Logger.Debug(nameof(Game), ":starting new ball after tilting");
            StartNewBall();
        }

        /// <summary>
        /// Starts new ball in PinGod and invokes OnBallStarted on all Mode groups
        /// </summary>
        public virtual void StartNewBall()
        {
            //disable lamps, we're not using led in this game
            pinGod.DisableAllLamps();
            //start the ball
            pinGod.StartNewBall();
            //let all scenes that are in the Mode group that the ball has started
            pinGod.OnBallStarted(GetTree());
        }

        /// <summary>
        /// Uses Godots <see cref="Godot.Object.CallDeferred(string, object[])"/> to invoke <see cref="OnStartNewBall"/> on Tilt timeouts
        /// </summary>
        protected void timeout()
        {
            if (!_lastBall)
            {
                CallDeferred(nameof(OnStartNewBall));
            }
        }
    }
}
