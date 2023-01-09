#if TOOLS
using Godot;
using PinGod.Core;

namespace PinGod.AutoLoad 
{
    [Tool]
    public partial class AdjustmentsPlugin : EditorPlugin
    {
        const string ASSETS_DIR = "res://addons/assets/";
        const string ROOT_DIR = "res://addons/autoload/Adjustments/";
        const string VERSION = "1.0";

        /// <summary>
        /// Init plugin if in editor.
        /// </summary>
        public override void _EnterTree()
        {
            base._EnterTree();
            Logger.Debug(nameof(AdjustmentsPlugin), nameof(_EnterTree), ":" + ROOT_DIR);
            if (!Engine.IsEditorHint())
            {
                Logger.Debug(nameof(AdjustmentsPlugin), ":" + nameof(_EnterTree));
            }
            else
            {
                var scenePath = ROOT_DIR + "Adjustments.tscn";
                AddAutoloadSingleton("Adjustments", scenePath);
                Logger.Info(nameof(AdjustmentsPlugin), $": Autoload {scenePath}. Access scene from node /root/Adjustments");
            }
        }

        public override void _Ready()
        {
            base._Ready();
            Logger.Debug(nameof(AdjustmentsPlugin), nameof(_Ready));
        }

        /// <summary>
        /// clean up goes here. remove custom types
        /// </summary>
        public override void _ExitTree()
        {
            base._ExitTree();
            if (!Engine.IsEditorHint())
            {
                Logger.Debug(nameof(AdjustmentsPlugin), nameof(_ExitTree));
            }
            else
            {
                Logger.Debug(nameof(AdjustmentsPlugin), $":{nameof(_ExitTree)} - editor, removing autoloads.");
                RemoveAutoloadSingleton("Adjustments");
            }
        }
    }
}


#endif