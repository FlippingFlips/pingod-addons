using Godot;
using PinGod.Core;

#if TOOLS
namespace PinGod.AutoLoad
{
    /// <summary>
    /// Add the autoload singleton, find override first in autoload or plugin default
    /// </summary>
    [Tool]
	public partial class PinGodGamePlugin : PinGodEditorPlugin
	{
        public PinGodGamePlugin() : base("res://addons/pingod-game/Node/", "PinGodGame") { }

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
                Logger.Debug(nameof(PinGodGamePlugin), $" looking for scene at {overrideScene}");
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
                    Logger.Warning("autoload for PinGodGame.tscn could be set.",
                    nameof(PinGodGamePlugin), "WARNING: failed to set autoload " + path);
                }
            }
        }
	}
}
#endif
