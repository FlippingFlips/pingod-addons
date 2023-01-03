using Godot;
using System;
using System.Collections.Generic;
using static PinGodBase;

/// <summary>
/// A basic attract mode that can start a game and cycle scenes with flippers. Add scenes into the "AttractLayers" in scene tree
/// </summary>
public partial class Attract : Node
{
	/// <summary>
	/// The amount of time to change a scene
	/// </summary>
	[Export] byte _scene_change_secs = SceneChangeTime;

	/// <summary>
	/// 
	/// </summary>
	[Export] float[] _sceneTimes = null;

	#region Fields
	const byte SceneChangeTime = 5;
	int _currentScene = 0;
	int _lastScene = 0;
	/// <summary>
	/// access to the <see cref="PinGodGame"/> singleton
	/// </summary>
	protected PinGodGame pinGod;
	List<CanvasItem> Scenes = new List<CanvasItem>();
	private Timer timer;
	#endregion

	/// <summary>
	/// Sets up timer for cycling scenes in the AttractLayers tree. Stops ball searching
	/// </summary>
	public override void _EnterTree()
	{		
        Logger.Debug(nameof(Attract), ":", nameof(_EnterTree));        
		timer = (GetNode("AttractLayerChangeTimer") as Timer);
		timer.WaitTime = _scene_change_secs;

		//var err = pinGod.Connect(nameof(PinGodBase.SwitchCommandEventHandler), new Callable(this, nameof(SwitchHandler)));
		//godot4 connecting signal
		if (pinGod ==null && HasNode("/root/PinGodGame"))
		{
            pinGod = (GetNode("/root/PinGodGame") as PinGodGame);
        }

        if (pinGod?.PinGodMachine != null)
            pinGod.PinGodMachine.SwitchCommand += PinGod_SwitchCommand;

        var nodes = GetNode("AttractLayers").GetChildren();
		//add as canvas items as they are able to Hide / Show
		foreach (var item in nodes)
		{
			var cItem = item as CanvasItem;
			if (cItem != null)
			{
				Scenes.Add(cItem);
			}
		}

		//stop the ball search
		pinGod?.PinGodMachine?.SetBallSearchStop();
	}

    private void PinGod_SwitchCommand(string name, byte index, byte value)
    {
        if (value > 0)
        {
            switch (index)
            {
                case 9: //l flipper
                    CallDeferred("ChangeLayer", true);
                    break;
                case 11://r flipper
                    CallDeferred("ChangeLayer", false);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Just logs attract loaded
    /// </summary>
    public override void _Ready()
	{
		Logger.Debug(nameof(Attract),":",nameof(_Ready));
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        timer?.Stop();
        Logger.Debug(nameof(Attract), ":", nameof(_ExitTree));
        if (pinGod?.PinGodMachine != null)
            pinGod.PinGodMachine.SwitchCommand -= PinGod_SwitchCommand;
    }

    private void StartGame()
    {
        var started = pinGod?.StartGame() ?? false;
        if (started)
        {
            OnGameStartedFromAttract();
        }
        Logger.Info(nameof(Attract), ":", nameof(StartGame), ":", started);
    }

	/// <summary>
	/// What scene index are we on
	/// </summary>
	/// <returns></returns>
    public int GetCurrentSceneIndex() => _currentScene;

	/// <summary>
	/// stops the attract cycle timer
	/// </summary>
    public virtual void OnGameStartedFromAttract() 
	{
        Logger.Debug(nameof(Attract), ":", nameof(OnGameStartedFromAttract));
        timer?.Stop();
	}

    /// <summary>
    /// Switches the scenes visibility on a timer. Plays lamp seq in VP
    /// </summary>
    private void _on_Timer_timeout()
	{
		CallDeferred("ChangeLayer", false);
	}

    /// <summary>
    /// Changes the attract layer. Cycles the AttractLayers in the scene
    /// </summary>
    /// <param name="reverse">Cycling in reverse?</param>
    public virtual void ChangeLayer(bool reverse = false)
	{
		if (Scenes?.Count < 1) return;

		timer.Stop();

		//check if lower higher than our attract layers
		_currentScene = reverse ? _currentScene - 1 : _currentScene + 1;
		Logger.Verbose(nameof(Attract), ":change layer reverse: ", reverse, " scene", _currentScene);

		_currentScene = _currentScene > Scenes?.Count - 1 ? 0 : _currentScene;
		_currentScene = _currentScene < 0 ? Scenes?.Count - 1 ?? 0 : _currentScene;

		//hide the last layer and show new index
		Scenes[_lastScene].Hide(); //Scenes[_lastScene].Visible = false;
		Scenes[_currentScene].Show();// Scenes[_currentScene].Visible = true;

		_lastScene = _currentScene;

		float delay = _scene_change_secs;
		if (_sceneTimes?.Length > 0)
        {
			if(_currentScene <= _sceneTimes.Length)
            {
				delay = _sceneTimes[_currentScene];
            }
        }

		timer.Start(delay);
	}
}
