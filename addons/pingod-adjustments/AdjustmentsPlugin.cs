#if TOOLS
using Godot;
using PinGod.Core;

namespace PinGod.AutoLoad 
{
    /// <summary> Adds Adjustments Autoload singleton when plugin enabled in godot editor</summary>
    [Tool]
    public partial class AdjustmentsPlugin : PinGodEditorPlugin
    {
        public AdjustmentsPlugin() : base ("res://addons/pingod-adjustments/", "Adjustments") { }

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
                Logger.Debug(nameof(AdjustmentsPlugin), $": looking for scene at {overrideScene}");
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
                    Logger.Warning("autoload for Adjustments.tscn could be set.",
                    nameof(AdjustmentsPlugin), "WARNING: failed to set autoload " + path);
                }
            }
        }
    }
}
#endif