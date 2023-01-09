using Godot;
using PinGod.Core;
using PinGod.Core.Service;

namespace PinGod.AutoLoad
{
    /// <summary>
    /// Window commands. Handles UI and actions.
    /// </summary>
    [Tool]
    public partial class WindowActionsPlugin : EditorPlugin
    {
        const string ASSETS_DIR = "res://addons/assets/";
        const string ROOT_DIR = "res://addons/autoload/WindowActions/";
        const string VERSION = "1.0";

        /// <summary>
        /// Initialization of the PlugIn. Adds CustomTypes new type with a name, a parent type, a script and an icon.
        /// </summary>
        public override void _EnterTree()
        {
            Logger.Debug(nameof(WindowActionsPlugin), nameof(_EnterTree), ": WINDOW ACTIONS - " + $"VERSION:{VERSION}");
            if (Engine.IsEditorHint())
            {
                /*
                using var texture = GD.Load<Texture2D>($"{ASSETS_DIR}img/pinball.png");
                var script = GD.Load<Script>(ROOT_DIR + nameof(PinGodWindowActionsNode) + ".cs");
                AddCustomType(nameof(PinGodWindowActionsNode), nameof(Node), script, texture);
                */

                var scenePath = $"res://autoload/WindowActions.tscn";
                Logger.Debug(nameof(WindowActionsPlugin), ":" + nameof(_EnterTree), $": looking for WindowActions scene....");
                if (Godot.FileAccess.FileExists(scenePath))
                {
                    AddAutoloadSingleton("WindowActions", scenePath);
                    Logger.Info(nameof(WindowActionsPlugin), $": Autoload " + scenePath, ". Access scene from node /root/WindowActions");
                }
                else if (Godot.FileAccess.FileExists(ROOT_DIR + "WindowActions.tscn"))
                {
                    AddAutoloadSingleton("WindowActions", ROOT_DIR + "WindowActions.tscn");
                    Logger.Info(nameof(WindowActionsPlugin), $": Autoloaded " + ROOT_DIR + "WindowActions.tscn", ". Access scene from node /root/WindowActions\nTo customize settings or add game switches duplicate the WindowActions.tscn to the autoload folder and re-enable plugin.");
                }
                else
                {
                    Logger.WarningRich("[color=yellow]", nameof(WindowActionsPlugin), ": Create scene file with " + nameof(WindowActionsNode) + " as a base script at" + scenePath, "[/color]");
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
            Logger.Debug(nameof(WindowActionsPlugin), ":" + nameof(_ExitTree), " removing types");
            //RemoveCustomType(nameof(PinGodWindowActionsNode));
            RemoveAutoloadSingleton("WindowActions");
        }
    }
}