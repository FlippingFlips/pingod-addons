using Godot;
using PinGod.Core;
using PinGod.Core.Service;

namespace PinGod.AutoLoad
{
    /// <summary>
    /// AudioManager node
    /// </summary>
    [Tool]
    public partial class AudioManagerPlugin : EditorPlugin
    {
        const string ASSETS_DIR = "res://addons/assets/";
        const string ROOT_DIR = "res://addons/autoload/AudioManager/";
        const string VERSION = "1.0";

        /// <summary>
        /// Initialization of the PlugIn. Adds CustomTypes new type with a name, a parent type, a script and an icon.
        /// </summary>
        public override void _EnterTree()
        {
            Logger.Debug(nameof(AudioManagerPlugin), nameof(_EnterTree), ":" + ROOT_DIR);
            if (Engine.IsEditorHint())
            {
                using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");
                var script = GD.Load<Script>("res://addons/pingod-core/Service/" + nameof(AudioManager) + ".cs");
                Logger.Debug(nameof(AudioManagerPlugin), ":" + nameof(_EnterTree), " loaded custom types");

                var scenePath = "res://autoload/AudioManager.tscn";
                if (FileAccess.FileExists(scenePath))
                {
                    AddAutoloadSingleton(nameof(AudioManager), scenePath);
                    Logger.Info(nameof(AudioManagerPlugin), $": Autoload {scenePath}. Access scene from node /root/AudioManager");
                }
                else
                {
                    scenePath = ROOT_DIR + "AudioManager.tscn";
                    AddAutoloadSingleton(nameof(AudioManager), scenePath);
                    Logger.Info(nameof(AudioManagerPlugin), $": Autoload {scenePath}. Access scene from node /root/AudioManager");
                    Logger.Info(nameof(AudioManagerPlugin), $": Add a scene in /autoload/AudioManager.tscn then re-enable the plugin\n Use the AutoManager.cs script or your own.");
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();
        }

        /// <summary>
        /// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
        /// </summary>
        public override void _ExitTree()
        {
            Logger.Debug(nameof(AudioManagerPlugin), ":" + nameof(_EnterTree), " removing types");
            RemoveCustomType(nameof(AudioManager));
            RemoveAutoloadSingleton(nameof(AudioManager));
        }
    }
}