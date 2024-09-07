using Godot;
using PinGod.Core;

#if TOOLS
namespace PinGod.AutoLoad
{
    /// <summary> AudioManagerPlugin enabled in plug-ins menu </summary>
    [Tool]
	public partial class AudioManagerPlugin : PinGodEditorPlugin
	{
        public AudioManagerPlugin() : base("res://addons/pingod-audio/Node/", "AudioManager") { }
        public override void _EnterTree() => base._EnterTree();
        public override void _ExitTree() => base._ExitTree();

        /// <summary>
        /// Add the autoload singleton, find override first in autoload or plugin default
        /// </summary>
        protected override void SetAutoLoadPlugin()
        {
            //assign the scene when in editor
            if (Engine.IsEditorHint())
            {
                var overrideScene = AutoLoadSceneOverridePath();
                Logger.Debug(nameof(AudioManagerPlugin), $" looking for scene at {overrideScene}");
                if (FileAccess.FileExists(overrideScene))
                {
                    AddAutoLoad(overrideScene);
                    return;
                }

                var path = AutoLoadScenePath();
                if (FileAccess.FileExists(path))
                {
                    AddAutoLoad(path);
                }
                else
                {
                    Logger.Warning("autoload for AudioManager.tscn could be set.",
                    nameof(AudioManagerPlugin), "WARNING: failed to set autoload " + path);
                }
            }
        }
    }
}
#endif
