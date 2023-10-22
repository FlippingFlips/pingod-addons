using Godot;
using PinGod.Core;

#if TOOLS
namespace PinGod.AutoLoad
{
    /// <summary> Adds MemoryMap Autoload singleton when plugin enabled in godot editor</summary>
    [Tool]
	public partial class MemoryMapPlugin : PinGodEditorPlugin
	{
        public MemoryMapPlugin() : base("res://addons/pingod-memorymap-win/", "MemoryMap") { }

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
                Logger.Debug(nameof(MemoryMapPlugin), $" looking for scene at {overrideScene}");
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
                else { Logger.Warning("autoload for MemoryMap.tscn could be set.", 
                    nameof(MemoryMapPlugin), "WARNING: failed to set autoload " + path); }
            }
        }
	}
}

#endif
