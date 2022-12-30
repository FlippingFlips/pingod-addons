using Godot;
using static PinGodBase;

/// <summary>
/// sends BumperHit signal, plays sound and coil if given
/// </summary>
public class Bumper : Node
{
    /// <summary>
    /// audio player
    /// </summary>
    public AudioStreamPlayer player;

    /// <summary>
    /// The stream to play when bumper hit
    /// </summary>
    [Export] AudioStream _AudioStream = null;

    /// <summary>
    /// Coil name
    /// </summary>
    [Export] string _CoilName = string.Empty;

    private PinGodGame _pinGod;

    /// <summary>
    /// Bumper Switch name
    /// </summary>
    [Export] string _SwitchName = string.Empty;
	/// <summary>
	/// Emitted on bumper input
	/// </summary>
	/// <param name="name"></param>
	[Signal] delegate void BumperHit(string name);
	/// <summary>
	/// Switches off input if no switch available. Sets audio stream
	/// </summary>
    public override void _EnterTree()
    {
		base._EnterTree();

		if (!Engine.EditorHint)
        {			
			_pinGod = GetNode<PinGodGame>("/root/PinGodGame");
			if (_pinGod == null) this.SetProcessInput(false);
			if (string.IsNullOrWhiteSpace(_SwitchName)) this.SetProcessInput(false);

			//update the player stream remove the player from the scene if dev hasn't loaded a stream
			player = GetNode<AudioStreamPlayer>(nameof(AudioStreamPlayer));
			if (_AudioStream != null)
			{
				player.Stream = _AudioStream;
			}
            //else { this.RemoveChild(player); player.QueueFree(); player = null; }

            _pinGod.Connect(nameof(SwitchCommand), this, nameof(SwitchCommandHandler));
        }
	}

    internal void SetAudioStream(AudioStream audioStream)
    {
        this._AudioStream = audioStream;
        player.Stream = this._AudioStream;
    }

    private void SwitchCommandHandler(string swName, byte index, byte value)
    {
        //exit no game in play or tilted
        if (!_pinGod.GameInPlay || _pinGod.IsTilted) return;

		if(_SwitchName == swName)
		{
			if (value > 0)
			{
				//play sound for bumper
				if (_AudioStream != null) { player.Play(); }

				//pulse coil
				if (!string.IsNullOrWhiteSpace(_CoilName)) { _pinGod.SolenoidPulse(_CoilName); }

				//publish hit event
				EmitSignal(nameof(BumperHit), _SwitchName);
			}
			else
			{ //switch off}}
			}
		}
    }
}