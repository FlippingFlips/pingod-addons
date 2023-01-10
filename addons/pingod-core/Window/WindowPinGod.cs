using Godot;
using PinGod.Core;

namespace PinGod.EditorPlugins
{
    public partial class WindowPinGod : Window
    {
        [Export] PackedScene _scene;

        public override void _Ready()
        {
            base._Ready();
            this.CloseRequested += PinGodWindow_CloseRequested;
        }

        private void PinGodWindow_CloseRequested()
        {
            GD.Print("close request");
            this.QueueFree();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            GD.Print($"{Name} exiting");
        }

        /// <summary>
        /// call this to instance the scene given into the window as child
        /// </summary>
        public void InitPackedScene()
        {
            if (_scene != null)
            {
                CallDeferred("add_child", _scene.Instantiate());
            }
            else { Logger.WarningRich(nameof(WindowPinGod), "[color=yellow]: no scene was set for the window: " + Title, "[/color]"); }
        }
    }
}