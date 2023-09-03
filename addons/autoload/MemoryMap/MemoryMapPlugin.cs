using Godot;
using PinGod.Core;

#if TOOLS
namespace PinGod.AutoLoad
{
	/// <summary>
	/// This add custom types to use in the Godot editor
	/// </summary>
	[Tool]
	public partial class MemoryMapPlugin : EditorPlugin
	{
		const string ASSETS_DIR = "res://addons/assets/";
		const string ROOT_DIR = "res://addons/autoload/MemoryMap/";

		/// <summary>
		/// Create a custom type for Create New Node
		/// </summary>
		public override void _EnterTree()
		{
			base._EnterTree();
			Logger.Debug(nameof(MemoryMapPlugin), nameof(_EnterTree), ":" + ROOT_DIR);

			//option to add as auto loaded with the scene, scene needs to be configured
			var path = "res://autoload/MemoryMap.tscn";
			if (FileAccess.FileExists(path))
			{
				AddAutoloadSingleton("MemoryMap", path);
				Logger.Debug(nameof(MemoryMapPlugin), nameof(_EnterTree), ": autoload MemoryMap scene found at " + path);
			}
			else
			{
				path = ROOT_DIR + "MemoryMap.tscn";
				AddAutoloadSingleton("MemoryMap", path);
				Logger.Debug(nameof(MemoryMapPlugin), nameof(_EnterTree), ": autoload MemoryMap scene found at " + path);
			}
		}

		public override void _Ready()
		{
			base._Ready();
			Logger.Debug(nameof(MemoryMapPlugin), nameof(_Ready));
		}

		/// <summary>
		/// Clean-up of the PlugIn goes here. Always remember to remove it from the engine when deactivated.
		/// </summary>
		public override void _ExitTree()
		{
			base._ExitTree();
			if (!Engine.IsEditorHint())
			{
				Logger.Debug(nameof(MemoryMapPlugin), $":{nameof(_ExitTree)}");
			}
			else
			{
				Logger.Debug(nameof(MemoryMapPlugin), $":{nameof(_ExitTree)} - editor, removing custom type");
			}

			//RemoveCustomType(nameof(PinGodMemoryMapNode));
			RemoveAutoloadSingleton("MemoryMap");
		}
	}
}

#endif
