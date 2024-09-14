using Godot;
using PinGod.Core.Service;

/// <summary> Sends BumperHit signal, plays sound and coil if given</summary>
[Tool]
public partial class Bumper : PinGodGameNode
{
    private MachineNode _machine;

    private IPinGodGame _pinGod;

    /// <summary>
    /// The stream to play when bumper hit
    /// </summary>
    [Export] AudioStream _AudioStream = null;

    /// <summary> Coil name </summary>
    [Export] string _CoilName = string.Empty;

    /// <summary> Bumper Switch name </summary>
    [Export] string _SwitchName = string.Empty;

    /// <summary> Optional audio player</summary>
    [Export] AudioStreamPlayer player = new();

    /// <summary> Emitted on bumper input</summary>
    /// <param name="name"></param>
    [Signal] public delegate void BumperHitEventHandler(string name);

    /// <summary>
    /// Switches off input if no switch available. Sets audio stream
    /// </summary>
    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
            base._EnterTree();

            //add the audio stream player
            AddChild(player);

            //disable processing?
            if (_pinGod == null) this.SetProcessInput(false);
            if (string.IsNullOrWhiteSpace(_SwitchName)) this.SetProcessInput(false);

            //update the player stream remove the player from the scene if dev hasn't loaded a stream
            if (HasNode(nameof(AudioStreamPlayer)))
            {
                player = GetNode<AudioStreamPlayer>(nameof(AudioStreamPlayer));
            }

            //remove the player if we don't have a sound
            if (_AudioStream != null && player != null)
            {
                player.Stream = _AudioStream;
            }
            else { this.RemoveChild(player); player.QueueFree(); player = null; }

            //get switch event from Machine
            if (HasNode(Paths.ROOT_MACHINE))
            {
                _machine = GetNode<MachineNode>(Paths.ROOT_MACHINE);
                _machine.SwitchCommand += SwitchCommandHandler;
            }
        }
    }

    public void SetAudioStream(AudioStream audioStream)
    {
        this._AudioStream = audioStream;
        player.Stream = this._AudioStream;
    }

    /// <summary> doesn't pulse coil </summary>
    /// <param name="swName"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void SwitchCommandHandler(string swName, byte index, byte value)
    {
        //exit no game in play or tilted
        if (!_pinGod.GameInPlay || _pinGod.IsTilted) return;

        if (_SwitchName == swName)
        {
            if (value > 0)
            {
                //play sound for bumper
                if (_AudioStream != null) { player.Play(); }

                //don't bother to pulse coil, VP does this
                //if (!string.IsNullOrWhiteSpace(_CoilName)) { _machine?.CoilPulse(_CoilName); }

                //publish hit event
                EmitSignal(nameof(BumperHit), _SwitchName);
            }
            else
            { //switch off}}
            }
        }
    }
}