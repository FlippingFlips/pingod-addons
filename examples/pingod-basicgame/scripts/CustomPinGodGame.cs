using PinGod.Core;
using PinGod.Core.Game;
using PinGod.Game;

/// <summary>
/// Custom class of <see cref="PinGodGame"/> to override when a player is added
/// </summary>
public partial class CustomPinGodGame : PinGodGame
{
	/// <summary>
	/// Processing is disabled when _resources?.IsLoading() is complete. For first game run. <para/>
	/// The <see cref="OnResourcesLoaded"/> will load the attract mode when completed
	/// </summary>
	/// <param name="_delta"></param>
	public override void _Process(double _delta)
	{
		base._Process(_delta);
		if (_resources != null)
		{
			bool result = _resources?.IsLoading() ?? true;
			if (!result)
			{
				//resources loaded
				SetProcess(false);
				OnResourcesLoaded();
			}
		}
	}

	/// <summary>
	/// When the resource node is fully loaded we gets the MainScene and load the attract into it
	/// </summary>
	private void OnResourcesLoaded()
	{
		var ms = GetNodeOrNull<MainScene>("/root/MainScene");
		if (ms != null)
		{
			Logger.WarningRich(nameof(CustomPinGodGame), ":Resources loaded");
			ms.AddAttract();
		}
		else { Logger.WarningRich(nameof(CustomPinGodGame), "[color=yellow] no MainScene found.[/color]"); }
	}

	/// <summary>
	/// override to create our own player type for this game
	/// </summary>
	/// <param name="name"></param>
	public override void CreatePlayer(string name) => Players.Add(new BasicGamePlayer() { Name = name, Points = 0 });


	/// <summary>
	/// Logs when this class is setup, nothing more.
	/// </summary>
	public override void Setup()
	{
		base.Setup();
		LogInfo(nameof(CustomPinGodGame), ":setup custom game finished");

		//get the root viewport
		GetTree().Root.SizeChanged += on_size_changed;
	}

	private void on_size_changed() => Logger.Verbose(nameof(CustomPinGodGame), ":size changed");
}
