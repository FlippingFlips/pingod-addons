using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A basic attract mode that can start a game and cycle scenes with flippers. Add scenes into the "AttractLayers" in scene tree
/// </summary>
public class Attract : Node
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
        pinGod = (GetNode("/root/PinGodGame") as PinGodGame);
		timer = (GetNode("AttractLayerChangeTimer") as Timer);
		timer.WaitTime = _scene_change_secs;

		pinGod.Connect(nameof(PinGodBase.SwitchCommand), this, nameof(SwitchHandler));

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

		pinGod.SetBallSearchStop();
	}

    private void SwitchHandler(string name, byte index, byte value)
    {        
        if (value > 0)
		{
			switch (index)
			{
				case 1:
                case 2:
                case 3: //Coin buttons. See PinGod.vbs for Standard switches
                    pinGod.AudioManager.PlaySfx("credit");
                    pinGod.AddCredits((byte)(1 * index));
					break;
                case 9: //l flipper
                    CallDeferred("ChangeLayer", true);
					break;
                case 11://r flipper
                    CallDeferred("ChangeLayer", false);
                    break;
				case 19://start
                    CallDeferred(nameof(StartGame));
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

    private void StartGame()
    {
        var started = pinGod.StartGame();
        if (started)
        {
            OnGameStartedFromAttract();
        }
        Logger.Debug(nameof(Attract), ":", nameof(StartGame), ":", started);
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
        timer.Stop();
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
		Logger.Debug(nameof(Attract), ":change layer reverse: ", reverse, " scene", _currentScene);

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
