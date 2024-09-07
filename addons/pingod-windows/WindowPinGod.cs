using Godot;
using PinGod.Core;

namespace PinGod.EditorPlugins
{
	/// <summary>Window that can load a given packed scene when InitPackedScene is used</summary>
	public partial class WindowPinGod : Window
	{
        /// <summary>A packed scene to load into the window if given</summary>
        [Export] protected PackedScene _scene;

		/// <summary>This override just logs the exit</summary>
		public override void _ExitTree()
		{
			base._ExitTree();
			Logger.Debug(nameof(WindowPinGod), ":exiting");
		}

        /// <summary>Runs <see cref="InitPackedScene"/> and adds it as a child to this windows tree</summary>
        public override void _EnterTree()
        {
            base._EnterTree();
            InitPackedScene();
        }

        public override void _Ready()
		{
			base._Ready();
			this.CloseRequested += PinGodWindow_CloseRequested;
		}

        /// <summary>This removes the window from the tree, clears it completely</summary>
        public virtual void PinGodWindow_CloseRequested()
		{
			Logger.Debug(nameof(WindowPinGod), ":close request");
			this.QueueFree();
		}

		/// <summary>call this to instance the scene given into the window as child</summary>
		public virtual void InitPackedScene()
		{
			if (_scene != null)
			{
				if(this.GetChildCount() == 0)
					CallDeferred("add_child", _scene.Instantiate());
			}
			else { Logger.WarningRich(nameof(WindowPinGod), "[color=yellow]: no scene was set for the window: " + Title, "[/color]"); }
		}
	}
}
