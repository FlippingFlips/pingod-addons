using Godot;
using PinGod.Core;
using PinGod.Core.Service;
using PinGod.Modes;

namespace PinGod.Game
{
    /// <summary>
    /// Custom Game class for Basic Game
    /// </summary>
    public partial class Game : PinGodGameNode, IGame
    {
        /// <summary>
        /// Default scene file to use for Multi-Ball
        /// </summary>
        [Export(PropertyHint.File)] protected string MULTIBALL_SCENE = "res://addons/modes/multiball/Multiball.tscn";

        private bool _lastBall;
        private Timer _tiltedTimeOut;
        private Bonus endOfBallBonus;
        private PackedScene multiballPkd;
        private ScoreEntry scoreEntry;

        /// <summary>
        /// Connects signals to basic game events, handles tilt time outs
        /// </summary>
        public override void _EnterTree()
        {
            base._EnterTree();

            Logger.Debug(nameof(Game), ":", nameof(_EnterTree), ":looking for ScoreEntry and Bonus scenes..");
            scoreEntry = GetNodeOrNull<ScoreEntry>("Modes/ScoreEntry");
            endOfBallBonus = GetNodeOrNull<Bonus>("Modes/Bonus");

            Logger.Debug(nameof(Game), ":loading multiball scene:  " + MULTIBALL_SCENE);
            multiballPkd = ResourceLoader.Load(MULTIBALL_SCENE) as PackedScene;

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
            base._ExitTree();
            Logger.Debug(nameof(Game), ":", nameof(_ExitTree));
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
        /// Add a display at end of ball
        /// </summary>
        public virtual void OnBallEnded(bool lastBall)
        {
            Logger.Info(nameof(Game), ":ball ended", pinGod.BallInPlay, " last ball:" + lastBall);
            _lastBall = lastBall;

            EndMultiball();

            if (!pinGod.IsTilted)
            {
                pinGod.InBonusMode = true;
                Logger.Info(nameof(Game), ":adding bonus scene for player: " + pinGod.CurrentPlayerIndex);
                endOfBallBonus?.StartBonusDisplay();
                return;
            }
            else if (pinGod.IsTilted && lastBall)
            {
                Logger.Debug(nameof(Game), ":last ball in tilt");
                pinGod.InBonusMode = false;
                scoreEntry?.DisplayHighScore();
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
        /// Displays high score if this is the last ball.
        /// </summary>
        public virtual void OnBonusEnded()
        {
            Logger.Debug(nameof(Game), ":bonus ended, starting new ball");
            pinGod.InBonusMode = false;
            if (_lastBall)
            {
                Logger.Debug(nameof(Game), ":last ball played, end of game");
                scoreEntry?.DisplayHighScore();
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
        /// Signals to Mode groups OnBallSaved
        /// </summary>
        public virtual void OnBallSaved() => pinGod.OnBallSaved(GetTree());

        void OnStartNewBall()
        {
            Logger.Debug(nameof(Game), ":starting new ball after tilting");
            StartNewBall();
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
