using Godot;
using PinGod.Base;
using System;

namespace PinGod.Core.Service
{
	/// <summary>
	/// Access the adjustments and audits for machine
	/// </summary>
	public partial class AdjustmentsNode : Node
	{
		public Adjustments _adjustments;
		public Audits _audits;

		/// <summary>
		/// display adjustments from the project
		/// </summary>
		private DisplaySettings _projectDisplaySettings;

		public override void _EnterTree()
		{
			base._EnterTree();
			if (!Engine.IsEditorHint())
			{
				Logger.Debug(nameof(AdjustmentsNode), ": loading standard adjustment model.");
				LoadAdjustments<Adjustments>();
				_audits = Audits.Load();
			}
		}

		public override void _ExitTree()
		{
			if (!Engine.IsEditorHint())
			{
				Logger.Debug(nameof(AdjustmentsNode), ":", nameof(_ExitTree), ": saving audits and adjustments");
				//SaveWindow
				var size = DisplayServer.WindowGetSize();
				var pos = DisplayServer.WindowGetPosition();

				if (_adjustments?.Display != null)
				{
					_adjustments.Display.X = pos.X;
					_adjustments.Display.Y = pos.Y;
					_adjustments.Display.Width = size.X;
					_adjustments.Display.Height = size.Y;
				}

				Adjustments.Save(_adjustments);
				Audits.Save(_audits);
			}
		}

		/// <summary>
		/// Loads or create adjustments. If no settings.save found then creates new using the display settings from the project, sets defaults.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="r"></param>
		public virtual void LoadAdjustments<T>() where T : Adjustments
		{
			Logger.Debug(nameof(AdjustmentsNode), ": ", nameof(LoadAdjustments), ": loading project display settings and adjustments. settings.save");

			_adjustments = Adjustments.Load<T>();
			//should be null when no save has been created
			if (_adjustments == null)
			{
				_adjustments = Activator.CreateInstance<T>();
				_projectDisplaySettings = Display.GetDisplayProjectSettings();
				_adjustments.Display = _projectDisplaySettings;
				_adjustments.Display.WidthDefault = _adjustments.Display.Width;
				_adjustments.Display.HeightDefault = _adjustments.Display.Height;
				Logger.Info(nameof(Adjustments), ": adjustments loaded from project settings");
			}
		}

		/// <summary>
		/// Loads custom adjustments type as Adjustments
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="r"></param>
		public virtual void SaveAdjustments<T>() where T : Adjustments
		{
			Logger.Debug(nameof(AdjustmentsNode), ": ", nameof(SaveAdjustments));
			Adjustments.Save<T>((T)_adjustments);
		}
	}
}
