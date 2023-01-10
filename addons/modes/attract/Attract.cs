using Godot;
using PinGod.Core;
using System.Collections.Generic;

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
        int _lastScene = 0;
        /// <summary>
        /// Scenes to cycle through
        /// </summary>
        List<CanvasItem> Scenes = new List<CanvasItem>();
        private Timer timer;
        #endregion

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
            }

            if (pinGod?.MachineNode != null)
                pinGod.MachineNode.SwitchCommand += OnPinGodSwitchCommand;
            else { Logger.WarningRich(nameof(Attract), ":[color=yellow]", "IPinGodGame wasn't found in root, no machine switch handlers were added[/color]"); }

            //stop the ball search
            pinGod?.MachineNode?.SetBallSearchStop();

            timer = (GetNode("AttractLayerChangeTimer") as Timer);
            timer.WaitTime = _scene_change_secs;
        }

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
        /// Switches the scenes visibility on a timer. Plays lamp seq in VP
        /// </summary>
        private void _on_Timer_timeout()
        {
            CallDeferred("ChangeLayer", false);
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
