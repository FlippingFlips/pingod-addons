using Godot;
using PinGod.Base;
using PinGod.Core;
using System.Collections.Generic;
using System.Linq;

namespace PinGod.Modes
{
    /// <summary>
    /// A basic attract mode that can start a game and cycle scenes with flippers. Add scenes into the "AttractLayers" in scene tree
    /// </summary>
    public partial class Attract : Node
    {
        /// <summary>
        /// The amount of time to change a scene
        /// </summary>
        [Export] byte _scene_change_secs = SceneChangeTime;

        /// <summary>
        /// 
        /// </summary>
        [Export] float[] _sceneTimes = null;

        #region Fields
        /// <summary>
        /// access to the <see cref="PinGodGame"/> singleton
        /// </summary>
        protected IPinGodGame pinGod;

        const byte SceneChangeTime = 5;
        int _currentScene = 0;
        protected List<HighScore> _highScoresList;
        int _lastScene = 0;
        /// <summary>
        /// Scenes to cycle through
        /// </summary>
        List<CanvasItem> Scenes = new List<CanvasItem>();
        private Timer timer;
        #endregion

        #region Godot Overrides
        /// <summary>
        /// Gets the AttractLayers from the scene
        /// </summary>
        public override void _EnterTree()
        {
            Logger.Debug(nameof(Attract), ":", nameof(_EnterTree));

            //add as canvas items as they are able to Hide / Show
            var nodes = GetNode("AttractLayers").GetChildren();
            foreach (var item in nodes)
            {
                var cItem = item as CanvasItem;
                if (cItem != null)
                {
                    Scenes.Add(cItem);
                }
            }
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            timer?.Stop();
            Logger.Debug(nameof(Attract), ":", nameof(_ExitTree));
            if (pinGod?.MachineNode != null)
                pinGod.MachineNode.SwitchCommand -= OnPinGodSwitchCommand;
        }

        /// <summary>
        /// Sets up timer for cycling scenes in the AttractLayers tree. Stops ball searching
        /// </summary>
        public override void _Ready()
        {
            Logger.Debug(nameof(Attract), ":", nameof(_Ready));

            if (pinGod == null && HasNode("/root/PinGodGame"))
            {
                pinGod = (GetNode("/root/PinGodGame") as IPinGodGame);
                //update any credits labels
                pinGod.EmitSignal("CreditAdded", pinGod.Credits);
            }

            if (pinGod?.MachineNode != null)
                pinGod.MachineNode.SwitchCommand += OnPinGodSwitchCommand;
            else { Logger.WarningRich(nameof(Attract), ":[color=yellow]", "IPinGodGame wasn't found in root, no machine switch handlers were added[/color]"); }

            //stop the ball search
            pinGod?.MachineNode?.SetBallSearchStop();

            timer = (GetNode("AttractLayerChangeTimer") as Timer);
            timer.WaitTime = _scene_change_secs;

            GetHighScores();
            UpdateHighScores();
        }
        #endregion

        /// <summary>
        /// Switches the scenes visibility on a timer by calling <see cref="ChangeLayer"/>
        /// </summary>
        public virtual void _on_Timer_timeout() => CallDeferred(nameof(ChangeLayer), false);

        /// <summary>
        /// Changes the attract layer. Cycles the AttractLayers in the scene
        /// </summary>
        /// <param name="reverse">Cycling in reverse?</param>
        public virtual void ChangeLayer(bool reverse = false)
        {
            if (Scenes?.Count < 1) return;

            timer.Stop();

            //check if lower higher than our attract layers
            _currentScene = reverse ? _currentScene - 1 : _currentScene + 1;
            Logger.Verbose(nameof(Attract), ":change layer reverse: ", reverse, " scene", _currentScene);

            _currentScene = _currentScene > Scenes?.Count - 1 ? 0 : _currentScene;
            _currentScene = _currentScene < 0 ? Scenes?.Count - 1 ?? 0 : _currentScene;

            //hide the last layer and show new index
            Scenes[_lastScene].Hide(); //Scenes[_lastScene].Visible = false;
            Scenes[_currentScene].Show();// Scenes[_currentScene].Visible = true;

            _lastScene = _currentScene;

            float delay = _scene_change_secs;
            if (_sceneTimes?.Length > 0)
            {
                if (_currentScene <= _sceneTimes.Length)
                {
                    delay = _sceneTimes[_currentScene];
                }
            }

            timer.Start(delay);
        }

        /// <summary>
        /// What scene index are we on
        /// </summary>
        /// <returns></returns>
        public int GetCurrentSceneIndex() => _currentScene;

        /// <summary>
        /// Gets the latest top scores to display in the high scores. Override this when need another high score source
        /// </summary>
        public virtual void GetHighScores()
        {
            _highScoresList = new List<HighScore>();
            if (pinGod?.Audits?.HighScores != null)
            {
                _highScoresList = pinGod.Audits.HighScores.Select(x => new HighScore { Name = x.Name, Points = x.Points }).ToList();
                Logger.Info(nameof(Attract), ":", nameof(GetHighScores), ": high scores found in adjustments.");
            }
        }

        /// <summary>
        /// stops the attract cycle timer
        /// </summary>
        public virtual void OnGameStartedFromAttract()
        {
            Logger.Debug(nameof(Attract), ":", nameof(OnGameStartedFromAttract));
            timer?.Stop();
        }

        /// <summary>
        /// Handles the flippers on 9 and 11 to change the attract layers
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public virtual void OnPinGodSwitchCommand(string name, byte index, byte value)
        {
            if (value > 0)
            {
                switch (index)
                {
                    case 9: //l flipper
                        CallDeferred("ChangeLayer", true);
                        break;
                    case 11://r flipper
                        CallDeferred("ChangeLayer", false);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Adds high scores to the high scores scene if available at %HighScores . (percent is for unique name so child can be anywhere in scene)
        /// </summary>
        public virtual void UpdateHighScores()
        {
            if (!HasNode("%HighScores")) return;
            if(_highScoresList?.Count > 0)
            {
                foreach (var score in _highScoresList)
                {
                    GetNode("%HighScores")?.Call("AddHighScore", score.Name, score.Points);
                }
                GetNode("%HighScores")?.Call("UpdateScoresText");
            }
            else { Logger.WarningRich(nameof(Attract), ": [color=yellow]No high scores were found to display in high scores scene.[/color]"); }
        }

        private void StartGame()
        {
            var started = pinGod?.StartGame() ?? false;
            if (started)
            {
                OnGameStartedFromAttract();
            }
            Logger.Info(nameof(Attract), ":", nameof(StartGame), ":", started);
        }
    }
}

