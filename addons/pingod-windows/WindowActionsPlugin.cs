using Godot;
using PinGod.Core;
using PinGod.Core.Service;

#if TOOLS

namespace PinGod.AutoLoad
{
    /// <summary> Add WindowActions singleton autoload. Handles UI and actions. </summary>
    [Tool]
	public partial class WindowActionsPlugin : PinGodEditorPlugin
	{        
        public WindowActionsPlugin() : base("res://addons/pingod-windows/", "WindowActions") { }

        public override void _EnterTree() => base._EnterTree();
        public override void _ExitTree() => base._ExitTree();

        public override void _Ready()
        {
            base._Ready();
            Logger.Debug(nameof(WindowActionsPlugin), nameof(_Ready));
        }
        
        protected override void SetAutoLoadPlugin()
        {
            //assign the scene when in editor
            if (Engine.IsEditorHint())
            {
                var overrideScene = AutoLoadSceneOverridePath();
                Logger.Debug(nameof(WindowActionsPlugin), $" looking for scene at {overrideScene}");
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
                else { Logger.Warning($"autoload for {base.Name} could be set.",
                    nameof(WindowActionsNode), "WARNING: failed to set autoload " + path); }
            }

        }
	}
}
#endif
