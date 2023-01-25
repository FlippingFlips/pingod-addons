using Godot;
using PinGod.Game;
using PinGod.Modes;

public partial class MsGame : Game
{
	private bool _lastBall;
	private Timer _tiltedTimeOut;
	private PackedScene multiballPkd;
	private ScoreEntry scoreEntry;
    private MsPinGodGame _mspinGod;

    #region Moon Station Game Specific
    public byte[] MoonTargets { get; internal set; }	
	public byte[] StationTargets { get; internal set; }
	#endregion

	public override void _EnterTree()
	{
		base._EnterTree();
		_mspinGod = pinGod as MsPinGodGame;
		pinGod.LogInfo("game: enter tree");
	}

	public void UpdateLamps()
	{
		if (_mspinGod.Multiplier > 1)
		{
            _mspinGod.SetLampState("multiplier_2", 2);

			if (_mspinGod.Multiplier > 2)
			{
                _mspinGod.SetLampState("multiplier_2", 1);
                _mspinGod.SetLampState("multiplier_3", 2);
			}
			if (_mspinGod.Multiplier > 3)
			{
                _mspinGod.SetLampState("multiplier_2", 1);
                _mspinGod.SetLampState("multiplier_3", 1);
                _mspinGod.SetLampState("multiplier_4", 2);
			}
		}
		else
		{
            _mspinGod.SetLampState("multiplier_2", 0);
            _mspinGod.SetLampState("multiplier_3", 0);
            _mspinGod.SetLampState("multiplier_4", 0);
		}
	}

	/// <summary>
	/// Sets <see cref="PinGodGameBase.IsMultiballRunning"/> to false and Any node that is in the multiball group is removed from tree
	/// </summary>
	public override void EndMultiball()
	{
		base.EndMultiball();
        _mspinGod.SolenoidOn("lampshow_1", 1);
	}

    public override void OnStartNewBall()
    {
        _mspinGod.LogInfo("game: starting ball after tilting");
        _mspinGod.SolenoidOn("lampshow_1", 0);
        base.OnStartNewBall();
    }
}
